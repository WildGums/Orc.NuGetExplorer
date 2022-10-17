namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
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
            _repositoryProvider = repositoryProvider;
            _packageMetadataProvider = packageMetadataProvider;
            _logger = logger;
        }

        public async Task<bool> PackageExistsAsync(IRepository packageRepository, string? filter, bool allowPrereleaseVersions)
        {
            var packages = await GetPackagesAsync(packageRepository, allowPrereleaseVersions, filter, 0, 1);

            return packages.Any();
        }

        public async Task<bool> PackageExistsAsync(IRepository packageRepository, string packageId)
        {
            var versionsMetadata = await _packageMetadataProvider.GetLowestLocalPackageMetadataAsync(packageId, true, CancellationToken.None);
            return versionsMetadata is not null;
        }

        public async Task<bool> PackageExistsAsync(IRepository packageRepository, IPackageDetails packageDetails)
        {
            var versionsMetadata = await _packageMetadataProvider.GetLocalPackageMetadataAsync(new PackageIdentity(packageDetails.Id, packageDetails.NuGetVersion), true, CancellationToken.None);
            return versionsMetadata is not null;
        }

        public async Task<IPackageDetails?> GetPackageAsync(IRepository packageRepository, string packageId, string version)
        {
            return await BuildMultiVersionPackageSearchMetadataAsync(packageId, version, true);
        }

        public async Task<IEnumerable<IPackageDetails>> GetPackagesAsync(IRepository packageRepository, bool allowPrereleaseVersions, string? filter = null, int skip = 0, int take = 10)
        {
            var sourceRepository = _repositoryProvider.CreateRepository(packageRepository.ToPackageSource());

            var searchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

            var searchFilters = new SearchFilter(allowPrereleaseVersions);

            var packages = await searchResource.SearchAsync(filter ?? string.Empty, searchFilters, skip, take, _logger, CancellationToken.None);

            //provide information about available versions
            var packageDetails = packages.Select(async package => await BuildMultiVersionPackageSearchMetadataAsync(package, sourceRepository, allowPrereleaseVersions))
                .Select(x => x.Result)
                .Where(result => result is not null);

            return packageDetails!;
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetVersionsOfPackageAsync(IRepository packageRepository, IPackageDetails package, bool allowPrereleaseVersions, int skip)
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

        private async Task<IPackageDetails?> BuildMultiVersionPackageSearchMetadataAsync(IPackageSearchMetadata packageSearchMetadata, SourceRepository sourceRepository, bool includePrerelease)
        {
            return await BuildMultiVersionPackageSearchMetadataAsync(packageSearchMetadata.Identity.Id, sourceRepository, includePrerelease);
        }

        private async Task<IPackageDetails?> BuildMultiVersionPackageSearchMetadataAsync(string packageId, SourceRepository sourceRepository, bool includePrerelease)
        {
            var versionsMetadata = await _packageMetadataProvider.GetPackageMetadataListAsync(packageId, includePrerelease, false, CancellationToken.None);

            var details = MultiVersionPackageSearchMetadataBuilder.FromMetadatas(versionsMetadata).Build() as IPackageDetails;

            return details;
        }

        private async Task<IPackageDetails?> BuildMultiVersionPackageSearchMetadataAsync(string packageId, string version, bool includePrerelease)
        {
            var versionsMetadata = await _packageMetadataProvider.GetPackageMetadataListAsync(packageId, includePrerelease, false, CancellationToken.None);

            var details = MultiVersionPackageSearchMetadataBuilder.FromMetadatas(versionsMetadata).Build(version) as IPackageDetails;

            return details;
        }
    }
}
