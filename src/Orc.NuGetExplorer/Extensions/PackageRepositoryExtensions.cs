// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageRepositoryExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using MethodTimer;
    using NuGet;

    internal static class PackageRepositoryExtensions
    {
        #region Methods
        [Time]
        public static IReadOnlyList<IPackage> FindAll(this IPackageRepository packageRepository, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            return packageRepository.FindFiltered(string.Empty, allowPrereleaseVersions, skip, take);
        }

        [Time]
        public static IReadOnlyList<IPackage> FindFiltered(this IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            switch (packageRepository)
            {
                case LazyLocalPackageRepository _:
                case LocalPackageRepository _:
                case AggregateRepository _:
                case DataServicePackageRepository _:
                    return packageRepository.FindFilteredManually(filter, allowPrereleaseVersions, skip, take).ToList();

                default:
                {
                    var queryable = packageRepository.Search(filter, allowPrereleaseVersions);

                    return queryable.OrderByDescending(x => x.DownloadCount).Skip(skip).Take(take).ToList();
                }
            }
        }

        private static IEnumerable<IPackage> FindFilteredManually(this IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            IEnumerable<IPackage> packages;

            switch (packageRepository)
            {
                case AggregateRepository aggregateRepository:
                    packages = aggregateRepository.Repositories
                        .SelectMany(x => x.CreateSearchQuery(filter, allowPrereleaseVersions, skip, take))
                        .OrderByDescending(x => x.DownloadCount)
                        .ToList();
                    break;

                default:
                    packages = packageRepository.CreateSearchQuery(filter, allowPrereleaseVersions, skip, take)
                        .ToList();
                    break;
            }

            var packageNames = packages
                .Select(x => x.Id).Distinct();
                
            foreach (var packageName in packageNames)
            {
                var packagesById = packageRepository.FindPackagesById(packageName);
                if (!allowPrereleaseVersions)
                {
                    packagesById = packagesById.Where(x => string.IsNullOrEmpty(x.Version.SpecialVersion));
                }

                var latestVersion = packagesById.Max(x => x.Version);

                yield return packageRepository.FindPackage(packageName, latestVersion);
            }
        }

        private static IQueryable<IPackage> CreateSearchQuery(this IPackageRepository packageRepository, string filter, bool allowPrereleaseVersions,
            int skip = 0, int take = 10)
        {
            var queryable = packageRepository.Search(filter, allowPrereleaseVersions)
                .Where(x => x.IsAbsoluteLatestVersion || x.IsLatestVersion);

            return queryable
                .OrderByDescending(x => x.DownloadCount)
                .Skip(skip)
                .Take(take);
        }

        [Time]
        public static IEnumerable<IPackage> FindPackageVersions(this IPackageRepository packageRepository, IPackage package, bool allowPrereleaseVersions,
            ref int skip, int minimalTake = 10)
        {
            Argument.IsNotNull(() => packageRepository);

            if (skip < 0)
            {
                return Enumerable.Empty<IPackage>();
            }

            var queryable = packageRepository.GetPackages().Where(x => Equals(x.Id, package.Id)).Skip(skip).Take(minimalTake);

            var result = new List<IPackage>(queryable.ToList());

            if (result.Count < minimalTake)
            {
                skip = -1;
            }
            else
            {
                skip += minimalTake;
            }

            if (!allowPrereleaseVersions && result.Any())
            {
                result = result.Where(x => x.IsReleaseVersion()).ToList();

                var count = result.Count;

                if (skip >= 0 && count < minimalTake)
                {
                    var additional = packageRepository.FindPackageVersions(package, false, ref skip, minimalTake).ToList();
                    result.AddRange(additional);
                }
            }

            return result;
        }
        #endregion
    }
}
