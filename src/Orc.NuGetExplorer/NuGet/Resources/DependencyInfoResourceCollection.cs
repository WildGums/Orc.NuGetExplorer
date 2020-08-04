namespace Orc.NuGetExplorer
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using global::NuGet.Protocol.Core.Types;
    using NuGet.Common;
    using NuGet.Frameworks;
    using NuGet.Packaging.Core;

    /// <summary>
    /// Wrapper against list of dependency recources from multiple repositories
    /// </summary>
    public class DependencyInfoResourceCollection : IEnumerable<DependencyInfoResource>
    {
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
            Argument.IsNotNull(() => resources);

            _resources = resources.ToList();
        }

        public DependencyInfoResourceCollection(DependencyInfoResource resource)
        {
            Argument.IsNotNull(() => resource);
            _resources = new List<DependencyInfoResource>() { resource };
        }

        public async Task<SourcePackageDependencyInfo> ResolvePackage(PackageIdentity package, NuGetFramework projectFramework, SourceCacheContext cacheContext, ILogger log, CancellationToken token)
        {
            foreach (var resource in _resources)
            {
                var packageDependencyInfo = await resource.ResolvePackage(package, projectFramework, cacheContext, log, token);

                //just returns first existed package 
                if (packageDependencyInfo != null)
                {
                    return packageDependencyInfo;
                }
            }

            return null;
        }

        public async Task<IEnumerable<SourcePackageDependencyInfo>> ResolvePackages(PackageIdentity package, NuGetFramework projectFramework, SourceCacheContext cacheContext, ILogger log, CancellationToken token)
        {
            HashSet<SourcePackageDependencyInfo> packageDependencyInfos = new HashSet<SourcePackageDependencyInfo>();

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
