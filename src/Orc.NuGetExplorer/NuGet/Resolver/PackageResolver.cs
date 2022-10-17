﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace Orc.NuGetExplorer.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Common;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using NuGet.Versioning;

    /// <summary>
    /// A core package dependency resolver.
    /// </summary>
    /// <remarks>Not thread safe</remarks>
    public class PackageResolver
    {
        /// <summary>
        /// Resolve a package closure
        /// </summary>
        public IEnumerable<SourcePackageDependencyInfo> Resolve(PackageResolverContext context, CancellationToken token)
        {
            var stopWatch = new Stopwatch();
            token.ThrowIfCancellationRequested();

            // validation 
            foreach (var requiredId in context.RequiredPackageIds)
            {
                if (!context.AvailablePackages.Any(p => StringComparer.OrdinalIgnoreCase.Equals(p.Id, requiredId)))
                {
                    throw new NuGetResolverInputException(string.Format(CultureInfo.CurrentCulture, "Unable to find package '{0}'. Existing packages must be restored before performing an install or update.", requiredId));
                }
            }

            var invalidExistingPackages = new List<string>();

            var installedPackages = context.PackagesConfig.Select(p => p.PackageIdentity).ToArray();

            // validate existing package.config for any invalid dependency
            foreach (var package in installedPackages)
            {
                var existingPackage =
                    context.AvailablePackages.FirstOrDefault(
                        p =>
                            StringComparer.OrdinalIgnoreCase.Equals(p.Id, package.Id) &&
                            p.Version.Equals(package.Version));

                if (existingPackage is not null)
                {
                    // check if each dependency can be satisfied with existing packages
                    var brokenDependencies = GetBrokenDependencies(existingPackage, installedPackages);

                    if (brokenDependencies is not null && brokenDependencies.Any())
                    {
                        invalidExistingPackages.AddRange(brokenDependencies.Select(dependency => FormatDependencyConstraint(existingPackage, dependency)));
                    }
                }
                else
                {
                    // check same package is being updated and we've a higher version then 
                    // ignore logging warning for that.
                    existingPackage =
                        context.AvailablePackages.FirstOrDefault(
                            p =>
                                StringComparer.OrdinalIgnoreCase.Equals(p.Id, package.Id) &&
                                VersionComparer.Default.Compare(p.Version, package.Version) > 0);

                    if (existingPackage is null)
                    {
                        var packageString = $"'{package.Id} {package.Version.ToNormalizedString()}'";
                        invalidExistingPackages.Add(packageString);
                    }
                }
            }
            // log warning message for all the invalid package dependencies
            if (invalidExistingPackages.Count > 0)
            {
                context.Log.LogWarning(
                    string.Format(
                        CultureInfo.CurrentCulture, "One or more unresolved package dependency constraints detected in the existing packages.config file. All dependency constraints must be resolved to add or update packages. If these packages are being updated this message may be ignored, if not the following error(s) may be blocking the current package operation: {0}",
                        string.Join(", ", invalidExistingPackages)));
            }

            // convert the available packages into ResolverPackages
            var resolverPackages = new List<ResolverPackage>();

            // pre-process the available packages to remove any packages that can't possibly form part of a solution
            var availablePackages = RemoveImpossiblePackages(context.AvailablePackages, context.RequiredPackageIds);

            foreach (var package in availablePackages)
            {
                var dependencies = context.DependencyBehavior == DependencyBehavior.Ignore ?
                    Enumerable.Empty<PackageDependency>() :
                    package.Dependencies ?? Enumerable.Empty<PackageDependency>();

                resolverPackages.Add(new ResolverPackage(package.Id, package.Version, dependencies, package.Listed, false));
            }

            // Sort the packages to make this process as deterministic as possible
            resolverPackages.Sort(PackageIdentityComparer.Default);

            // Keep track of the ids we have added
            var groupsAdded = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var grouped = new List<List<ResolverPackage>>();

            // group the packages by id
            foreach (var group in resolverPackages.GroupBy(e => e.Id, StringComparer.OrdinalIgnoreCase))
            {
                groupsAdded.Add(group.Key);

                var curSet = group.ToList();

                // add an absent package for non-targets
                // being absent allows the resolver to throw it out if it is not needed
                if (!context.RequiredPackageIds.Contains(group.Key, StringComparer.OrdinalIgnoreCase))
                {
                    curSet.Add(new ResolverPackage(id: group.Key, version: null, dependencies: null, listed: true, absent: true));
                }

                grouped.Add(curSet);
            }

            // find all needed dependencies, excluding manually ignored dependencies
            var dependencyIds = resolverPackages.Where(e => e.Dependencies is not null)
                .SelectMany(e => e.Dependencies.Select(d => d.Id)).Distinct(StringComparer.OrdinalIgnoreCase);

            //var ignoredDependencyIds = dependencyIds.

            foreach (var depId in dependencyIds)
            {
                // packages which are unavailable need to be added as absent packages
                // ex: if A -> B  and B is not found anywhere in the source repositories we add B as absent
                if (!groupsAdded.Contains(depId))
                {
                    groupsAdded.Add(depId);
                    grouped.Add(new List<ResolverPackage>() { new ResolverPackage(id: depId, version: null, dependencies: null, listed: true, absent: true) });
                }
            }

            token.ThrowIfCancellationRequested();

            // keep track of the best partial solution
            var bestSolution = Enumerable.Empty<ResolverPackage>();

            Action<IEnumerable<ResolverPackage>> diagnosticOutput = (partialSolution) =>
            {
                // store each solution as they pass through.
                // the combination solver verifies that the last one returned is the best
                bestSolution = partialSolution;
            };

            // Run solver
            var comparer = new ResolverComparer(context.DependencyBehavior, context.PreferredVersions, context.TargetIds);

            var sortedGroups = ResolverInputSort.TreeFlatten(grouped, context);

            var solution = CombinationSolver<ResolverPackage>.FindSolution(
                groupedItems: sortedGroups,
                itemSorter: comparer,
                shouldRejectPairFunc: ShouldRejectPackagePair,
                diagnosticOutput: diagnosticOutput);

            // check if a solution was found
            if (solution is not null)
            {
                var nonAbsentCandidates = solution.Where(c => !c.Absent);

                if (nonAbsentCandidates.Any())
                {
                    // topologically sort non absent packages
                    var sortedSolution = ResolverUtility.TopologicalSort(nonAbsentCandidates);

                    // Find circular dependency for topologically sorted non absent packages since it will help maintain cache of 
                    // already processed packages
                    var circularReferences = ResolverUtility.FindFirstCircularDependency(sortedSolution);

                    if (circularReferences.Any())
                    {
                        // the resolver is able to handle circular dependencies, however we should throw here to keep these from happening
                        throw new NuGetResolverConstraintException(
                            string.Format(CultureInfo.CurrentCulture, "Circular dependency detected '{0}'.",
                            string.Join(" => ", circularReferences.Select(package => $"{package.Id} {package.Version.ToNormalizedString()}"))));
                    }

                    // solution found!
                    stopWatch.Stop();
                    context.Log.LogMinimal(
                        string.Format("Resolving dependency information took {0}", DatetimeUtility.ToReadableTimeFormat(stopWatch.Elapsed)));
                    return sortedSolution.Where(x => !context.IgnoredIds.Contains(x.Id)).ToArray();
                }
            }

            // no solution was found, throw an error with a diagnostic message
            var message = ResolverUtility.GetDiagnosticMessage(bestSolution, context.AvailablePackages, context.PackagesConfig, context.TargetIds, context.PackageSources);
            throw new NuGetResolverConstraintException(message);
        }

        public async Task<List<SourcePackageDependencyInfo>> ResolveWithVersionOverrideAsync(PackageResolverContext context,
            IExtensibleProject project,
            DependencyBehavior dependencyBehavior,
            Action<IExtensibleProject, PackageReference> conflictResolveAction,
            CancellationToken cancellationToken)
        {
            var availablePackages = Resolve(context, cancellationToken);
            // note: probably this is not required
            var installablePackages = availablePackages
                        .Select(
                            x => context.AvailablePackages
                                .Single(p => PackageIdentityComparer.Default.Equals(p, x))).ToList();

            var incomingPackages = installablePackages.ToList();
            foreach (var package in incomingPackages)
            {
                var packageConflicts = context.PackagesConfig
                    .Where(reference => string.Equals(reference.PackageIdentity.Id, package.Id)
                    && reference.PackageIdentity.Version.CompareTo(package.Version, VersionComparison.VersionReleaseMetadata) != 0).ToList();

                // Note: workaround to make only one version of package appears in the same time. The correct package version set in packages.config
                // while local files handled by extensibility

                foreach (var conflict in packageConflicts)
                {
                    var needToFix = false;
                    var conflictedVersion = conflict.PackageIdentity.Version;

                    switch (dependencyBehavior)
                    {
                        case DependencyBehavior.HighestMinor:
                        case DependencyBehavior.HighestPatch:
                        case DependencyBehavior.Highest:
                            needToFix = conflictedVersion.CompareTo(package.Version, VersionComparison.VersionReleaseMetadata) < 0;
                            break;

                        case DependencyBehavior.Lowest:
                            needToFix = conflictedVersion.CompareTo(package.Version, VersionComparison.VersionReleaseMetadata) > 0;
                            break;

                        case DependencyBehavior.Ignore:
                            continue;
                    }

                    if (needToFix)
                    {
                        conflictResolveAction(project, conflict);
                    }
                    else
                    {
                        // cancel package installation
                        installablePackages.Remove(package);
                    }
                }
            }

            return installablePackages;
        }

        private static IEnumerable<PackageDependency> GetBrokenDependencies(SourcePackageDependencyInfo package, IEnumerable<PackageIdentity> packages)
        {
            foreach (var dependency in package.Dependencies)
            {
                var target = packages.FirstOrDefault(targetPackage => StringComparer.OrdinalIgnoreCase.Equals(targetPackage.Id, dependency.Id));

                if (!ResolverUtility.IsDependencySatisfied(dependency, target))
                {
                    yield return dependency;
                }
            }

            yield break;
        }

        private static string FormatDependencyConstraint(SourcePackageDependencyInfo package, PackageDependency dependency)
        {
            var range = dependency.VersionRange;
            var dependencyString = $"{dependency.Id} {range?.ToNonSnapshotRange().PrettyPrint() ?? string.Empty}";

            // A 1.0.0 dependency: B (= 1.5)не
            return $"'{package.Id} {package.Version.ToNormalizedString()} constraint: {dependencyString}'";
        }

        /// <summary>
        /// Remove packages that can't possibly form part of a solution
        /// </summary>
        private static IEnumerable<SourcePackageDependencyInfo> RemoveImpossiblePackages(IEnumerable<SourcePackageDependencyInfo> packages, ISet<string> mustKeep)
        {
            List<SourcePackageDependencyInfo> before;
            var after = new List<SourcePackageDependencyInfo>(packages);

            do
            {
                before = after;
                after = InnerPruneImpossiblePackages(before, mustKeep);
            }
            while (after.Count < before.Count);

            return after;
        }

        private static List<SourcePackageDependencyInfo> InnerPruneImpossiblePackages(List<SourcePackageDependencyInfo> packages, ISet<string> mustKeep)
        {
            if (packages.Count == 0)
            {
                return packages;
            }

            var dependencyRangesByPackageId = new Dictionary<string, IList<VersionRange>>(StringComparer.OrdinalIgnoreCase);

            //  (1) Adds all package Ids including leaf nodes that have no dependencies
            foreach (var package in packages)
            {
                if (!dependencyRangesByPackageId.ContainsKey(package.Id))
                {
                    dependencyRangesByPackageId.Add(package.Id, new List<VersionRange>());
                }
            }

            //  (2) Create a look-up of every dependency that refers to a particular package Id
            foreach (var package in packages)
            {
                foreach (var dependency in package.Dependencies)
                {
                    if (dependencyRangesByPackageId.TryGetValue(dependency.Id, out var dependencyVersionRanges))
                    {
                        dependencyVersionRanges.Add(dependency.VersionRange);
                    }
                }
            }

            //  (3) Per package Id combine all the dependency ranges into a wider 'worst-case' range
            var dependencyByPackageId = new Dictionary<string, VersionRange>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in dependencyRangesByPackageId)
            {
                dependencyByPackageId.Add(item.Key, VersionRange.Combine(item.Value));
            }

            //  (4) Remove any packages that fall out side of the worst case range while making sure not to remove the packages we must keep
            var result = packages.Where(
                package => dependencyByPackageId[package.Id].Satisfies(package.Version) || mustKeep.Contains(package.Id))
                .ToList();

            return result;
        }

        /// <summary>
        /// Check if two packages can exist in the same solution.
        /// This is used by the resolver.
        /// </summary>
        private static bool ShouldRejectPackagePair(ResolverPackage p1, ResolverPackage p2)
        {
            var p1ToP2Dependency = p1.FindDependencyRange(p2.Id);
            if (p1ToP2Dependency is not null)
            {
                return p2.Absent || !p1ToP2Dependency.Satisfies(p2.Version);
            }

            var p2ToP1Dependency = p2.FindDependencyRange(p1.Id);
            if (p2ToP1Dependency is not null)
            {
                return p1.Absent || !p2ToP1Dependency.Satisfies(p1.Version);
            }

            return false;
        }
    }
}
