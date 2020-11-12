namespace Orc.NuGetExplorer.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using NuGet.Common;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Packaging;
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
            _logger = logger;
        }

        public async Task<bool> PackageExists(IRepository packageRepository, string filter, bool allowPrereleaseVersions)
        {
            var packages = await GetPackagesAsync(packageRepository, allowPrereleaseVersions, filter, 0, 1);

            return packages.Any();
        }

        public async Task<bool> PackageExists(IRepository packageRepository, string packageId)
        {
            var versionsMetadata = await _packageMetadataProvider.GetLowestLocalPackageMetadataAsync(packageId, true, CancellationToken.None);
            return versionsMetadata != null;
        }

        public async Task<bool> PackageExists(IRepository packageRepository, IPackageDetails packageDetails)
        {
            var versionsMetadata = await _packageMetadataProvider.GetLocalPackageMetadataAsync(new PackageIdentity(packageDetails.Id, packageDetails.NuGetVersion), true, CancellationToken.None);
            return versionsMetadata != null;
        }

        public async Task<IPackageDetails> GetPackage(IRepository packageRepository, string packageId, string version)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());

            return await BuildMultiVersionPackageSearchMetadata(packageId, sourceRepository, true);
        }

        public async Task<IEnumerable<IPackageDetails>> GetPackagesAsync(IRepository packageRepository, bool allowPrereleaseVersions, string filter = null, int skip = 0, int take = 10)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());

            var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

            var searchFilters = new SearchFilter(allowPrereleaseVersions);

            var packages = await searchResource.SearchAsync(filter ?? string.Empty, searchFilters, skip, take, _logger, CancellationToken.None);

            //provide information about available versions
            var packageDetails = packages.Select(async package => await BuildMultiVersionPackageSearchMetadata(package, sourceRepository, allowPrereleaseVersions))
                .Select(x => x.Result)
                .Where(result => result != null);

            return packageDetails;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetVersionsOfPackage(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, int skip)
        {
            if (skip < 0)
            {
                return Enumerable.Empty<IPackageSearchMetadata>();
            }

            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());

            var getMetadataResult = await _packageMetadataProvider.GetPackageMetadataListAsync(package.Id, allowPrereleaseVersions, false, CancellationToken.None);

            var versions = getMetadataResult.Skip(skip).ToList();

            return versions;
        }

        private async Task<IPackageDetails> BuildMultiVersionPackageSearchMetadata(IPackageSearchMetadata packageSearchMetadata, SourceRepository sourceRepository, bool includePrerelease)
        {
            return await BuildMultiVersionPackageSearchMetadata(packageSearchMetadata.Identity.Id, sourceRepository, includePrerelease);
        }

        private async Task<IPackageDetails> BuildMultiVersionPackageSearchMetadata(string packageId, SourceRepository sourceRepository, bool includePrerelease)
        {
            var versionsMetadata = await _packageMetadataProvider.GetPackageMetadataListAsync(packageId, includePrerelease, false, CancellationToken.None);

            var details = MultiVersionPackageSearchMetadataBuilder.FromMetadatas(versionsMetadata).Build() as IPackageDetails;

            return details;
        }
    }
}
