namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Logging;
    using global::NuGet.Protocol.Core.Types;
    using MethodTimer;
    using NuGet.Common;
    using NuGet.Frameworks;
    using NuGet.Packaging.Core;
    using NuGet.ProjectModel;
    using NuGet.Versioning;

    /// <summary>
    /// Wrapper against list of dependency recources from multiple repositories
    /// </summary>
    public class DependencyInfoResourceCollection : IEnumerable<DependencyInfoResource>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IReadOnlyList<DependencyInfoResource> _resources;

        public IEnumerator<DependencyInfoResource> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        public DependencyInfoResourceCollection(IReadOnlyList<DependencyInfoResource> resources)
        {
            ArgumentNullException.ThrowIfNull(resources);

            _resources = resources.ToList();
        }

        public DependencyInfoResourceCollection(DependencyInfoResource resource)
        {
            ArgumentNullException.ThrowIfNull(resource);

            _resources = new List<DependencyInfoResource>
            {
                resource
            };
        }

        public async Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackagesWithVersionSatisfyRangeAsync(PackageIdentity package, VersionRange versionRange, NuGetFramework projectFramework, SourceCacheContext cacheContext,
            ILogger log, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(projectFramework);

            var singlePackage = await ResolvePackageAsync(package, projectFramework, cacheContext, log, token);

            // Check is this package satisfy requirements, if not, retrieve all dependency infos and find required package
            if (singlePackage is not null && versionRange.Satisfies(singlePackage.Version))
            {
                Log.Debug($"Found package {package} satisfying version range {versionRange}. Going to skip request of package with same identity");
                return new[] { singlePackage };
            }

            var packagesInRange = (await ResolvePackagesAsync(package, projectFramework, cacheContext, log, token)).Where(package => versionRange.Satisfies(package.Version)).ToList();
            return packagesInRange;
        }

        public async Task<SourcePackageDependencyInfo?> ResolvePackageAsync(PackageIdentity package, NuGetFramework projectFramework, SourceCacheContext cacheContext, ILogger log, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(projectFramework);

            foreach (var resource in _resources)
            {
                try
                {
                    var packageDependencyInfo = await resource.ResolvePackage(package, projectFramework, cacheContext, log, token);

                    //just returns first existed package 
                    if (packageDependencyInfo is not null)
                    {
                        return packageDependencyInfo;
                    }
                }
                catch (FatalProtocolException ex)
                {
                    // The resource cannot be unnaccessible of package metadata missed from feed
                    // Just log exception here and proceed, it contains enough info
                    Log.Warning(ex);
                }
            }

            return null;
        }

        [Time]
        public async Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackagesAsync(PackageIdentity package, NuGetFramework projectFramework, SourceCacheContext cacheContext, ILogger log, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(projectFramework);

            var packageDependencyInfos = new HashSet<SourcePackageDependencyInfo>();

            foreach (var resource in _resources)
            {
                var packageDependencyInfo = await resource.ResolvePackages(package.Id, projectFramework, cacheContext, log, token);

                foreach (var packageInfo in packageDependencyInfo)
                {
                    packageDependencyInfos.Add(packageInfo);
                }
            }

            return packageDependencyInfos;
        }
    }
}
