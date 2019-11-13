namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Common;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Providers;

    internal class PackageQueryService : IPackageQueryService
    {
        private readonly ISourceRepositoryProvider _repositoryProvider;
        private readonly IPackageMetadataProvider _packageMetadataProvider;
        private readonly ILogger _logger;

        public PackageQueryService(ISourceRepositoryProvider repositoryProvider, IPackageMetadataProvider packageMetadataProvider, ILogger logger)
        {
            Argument.IsNotNull(() => repositoryProvider);
            Argument.IsNotNull(() => packageMetadataProvider);
            Argument.IsNotNull(() => logger);

            _repositoryProvider = repositoryProvider;
            _packageMetadataProvider = packageMetadataProvider;
        }


        public int CountPackages(IRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());
            return 0;
        }

        public int CountPackages(IRepository packageRepository, string packageId)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());
            return 0;
        }

        public int CountPackages(IRepository packageRepository, IPackageDetails packageDetails)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());
            return 0;
        }

        public IPackageDetails GetPackage(IRepository packageRepository, string packageId, string version)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());
            return null;
        }

        public IEnumerable<IPackageDetails> GetPackages(IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());

            var searchResource = sourceRepository.GetResource<PackageSearchResource>();

            var searchFilters = new SearchFilter(allowPrereleaseVersions);

            using (var cts = new CancellationTokenSource())
            {
                var packages = searchResource.SearchAsync(filter ?? string.Empty, searchFilters, skip, take, _logger, cts.Token).Result;
            }

            //convert IPackageSearchMetadata => IPackageDetails
            return null;
        }

        public IEnumerable<IPackageDetails> GetVersionsOfPackage(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, ref int skip, int minimalTake = 10)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());
            return null;
        }
    }
}
