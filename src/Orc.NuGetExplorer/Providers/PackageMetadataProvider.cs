namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using Orc.FileSystem;
    using Orc.NuGetExplorer.Management;

    public class PackageMetadataProvider : IPackageMetadataProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ILogger NuGetLogger;

        private readonly IDirectoryService _directoryService;

        private readonly List<SourceRepository> _sourceRepositories;
        private readonly List<SourceRepository> _optionalLocalRepositories;
        private readonly SourceRepository _localDirectory;

        static PackageMetadataProvider()
        {
            NuGetLogger = ServiceLocator.Default.ResolveType<ILogger>();
        }

        /// <summary>
        /// The default ctor used by ITypeFactory to build instance
        /// </summary>
        /// <param name="sourceContext"></param>
        /// <param name="directoryService"></param>
        /// <param name="projectLocator"></param>
        /// <param name="sourceRepositoryProvider"></param>
        public PackageMetadataProvider(SourceContext sourceContext, IDirectoryService directoryService, IExtensibleProjectLocator projectLocator,
            ISourceRepositoryProvider sourceRepositoryProvider)
            : this(sourceContext, projectLocator.GetDefaultProject(), directoryService, sourceRepositoryProvider)
        {

        }

        public PackageMetadataProvider(SourceContext sourceContext, IExtensibleProject project, IDirectoryService directoryService, ISourceRepositoryProvider sourceRepositoryProvider)
        {
            Argument.IsNotNull(() => directoryService);
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => sourceRepositoryProvider);

            _directoryService = directoryService;

            var repositories = sourceContext.ReadAllSourceRepositories();
            var localRepository = project.AsSourceRepository(sourceRepositoryProvider);

            _sourceRepositories = new List<SourceRepository>();
            _optionalLocalRepositories = new List<SourceRepository>();

            if (repositories is not null)
            {
                _sourceRepositories.AddRange(repositories);
            }

            // Note: we don't use any optional repositories by default
            // _optionalLocalRepositories.AddRange(Array.Empty<SourceRepository>());

            _localDirectory = localRepository;
        }

        #region IPackageMetadataProvider
        /// <inheritdoc/>
        public async Task<IPackageSearchMetadata> GetPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            if (!_sourceRepositories.Any())
            {
                Log.Warning("No repositories available");
                return null;
            }

            var tasks = _sourceRepositories
               .Select(r => GetSinglePackageMetadataAsyncFromSourceAsync(r, identity, includePrerelease, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess)
                .Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);


            var master = completed.FirstOrDefault(m => !string.IsNullOrEmpty(m.Summary))
                ?? completed.FirstOrDefault()
                ?? PackageSearchMetadataBuilder.FromIdentity(identity).Build();

            //return master.WithVersions(
            //    asyncValueFactory: () => MergeVersionsAsync(identity, completed));

            return master;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataListAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
        {
            var tasks = _sourceRepositories.Select(repo => GetPackageMetadataListFromSourceAsync(repo, packageId, includePrerelease, includeUnlisted, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess).
                Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);

            var packages = completed.SelectMany(p => p);

            var uniquePackages = packages
                .GroupBy(
                   m => m.Identity.Version,
                   (v, ms) => ms.First());

            return uniquePackages;
        }

        // <inheritdoc/>
        public async Task<IPackageSearchMetadata> GetLocalPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            var sources = _optionalLocalRepositories.Append(_localDirectory);

            // Take the package from the first matched source
            foreach (var source in sources)
            {
                var result = await GetSinglePackageMetadataFromLocalSourceAsync(source, identity, cancellationToken);

                if (result is not null)
                {
                    //TODO test why additional fetching need?
                    //return result.WithVersions(
                    //    () => FetchAndMergeVersionsAsync(identity, includePrerelease, ))
                    return result;
                }
            }

            return null;
        }

        // <inheritdoc/>
        public async Task<IPackageSearchMetadata> GetLowestLocalPackageMetadataAsync(string packageid, bool includePrrelease, CancellationToken cancellationToken)
        {
            var sources = _optionalLocalRepositories.Append(_localDirectory);

            var tasks = sources.Select(r => GetPackageMetadataFromLocalSourceAsync(r, packageid, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess)
                .Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);

            var lowest = completed.SelectMany(p => p)
                .OrderBy(p => p.Identity.Version)
                .FirstOrDefault();

            return lowest;
        }

        #endregion


        [ObsoleteEx(ReplacementTypeOrMember = "PackageSearchMetadataExtensions.Highest", TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public async Task<IPackageSearchMetadata> GetHighestPackageMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken)
        {
            //returned type - packageRegistrationMetadata
            var metadataList = await GetPackageMetadataListAsync(packageId, includePrerelease, false, cancellationToken);

            var master = metadataList.OrderByDescending(x => x.Identity.Version).FirstOrDefault();

            return master?.WithVersions(() => metadataList.ToVersionInfo(includePrerelease));
        }

        /// <summary>
        /// Returns complete list of package metadata objects from specific source
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="packageId"></param>
        /// <param name="includePrerelease"></param>
        /// <param name="includeUnlisted"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataListFromSourceAsync(SourceRepository repository,
            string packageId,
            bool includePrerelease,
            bool includeUnlisted,
            CancellationToken cancellationToken)
        {

            var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

            using (var sourceCacheContext = new SourceCacheContext())
            {
                //todo
                //check httpCache created inside GetMetadataAsync()
                //The Root folder value didn't used when retry count is 0
                //Then temporary folder for package never created and SourceCacheContext dispose caused
                //DirectoryNotFoundException
                //var httpCache = HttpSourceCacheContext.Create(sourceCacheContext, 0);

                // Update http source cache context MaxAge so that it can always go online to fetch
                // latest versions of the package.
                //sourceCacheContext.MaxAge = DateTimeOffset.UtcNow;

                //force creating folder for cache even http retry count is 0
                _directoryService.Create(sourceCacheContext.GeneratedTempFolder);

                Log.Debug($"Get all versions metadata, creating temp {sourceCacheContext.GeneratedTempFolder}");

                var packages = await metadataResource?.GetMetadataAsync(
                    packageId,
                    includePrerelease,
                    includeUnlisted,
                    sourceCacheContext,
                    NuGetLogger,
                    cancellationToken);

                Log.Debug($"Returned package metadata count: {packages.Count()}");

                return packages;

            }
        }

        /// <summary>
        /// Returns list of all version metadata for single package from specific source
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="identity"></param>
        /// <param name="includePrerelease"></param>
        /// <param name="takeVersions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<IPackageSearchMetadata> GetSinglePackageMetadataAsyncFromSourceAsync(SourceRepository repository,
            PackageIdentity identity,
            bool includePrerelease,
            CancellationToken cancellationToken,
            bool takeVersions = true)
        {
            if (takeVersions)
            {
                //query all versions and pack them in a single object
                var versionsMetadatas = await GetPackageMetadataListFromSourceAsync(repository, identity.Id, includePrerelease, false, cancellationToken);

                if (!versionsMetadatas?.Any() ?? false)
                {
                    return null;
                }

                var unitedMetadata = versionsMetadatas
                    .FirstOrDefault(p => p.Identity.Version == identity.Version)
                    ?? PackageSearchMetadataBuilder.FromIdentity(identity).Build();

                return unitedMetadata.WithVersions(versionsMetadatas.ToVersionInfo(includePrerelease));
            }

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var metadataResource = await repository.GetResourceAsync<PackageMetadataResource>(cancellationToken);

                sourceCacheContext.MaxAge = DateTimeOffset.UtcNow;

                var package = await metadataResource?.GetMetadataAsync(identity, sourceCacheContext, NuGetLogger, cancellationToken);
                return package;
            }
        }


        private async Task<IPackageSearchMetadata> GetSinglePackageMetadataFromLocalSourceAsync(SourceRepository localRepository,
            PackageIdentity packageIdentity,
            CancellationToken token)
        {
            var localPackages = await GetPackageMetadataFromLocalSourceAsync(localRepository, packageIdentity.Id, token);

            var packageMetadata = localPackages?.FirstOrDefault(p => p.Identity.Version == packageIdentity.Version);

            var versions = new[]
            {
                new VersionInfo(packageIdentity.Version)
            };

            return packageMetadata?.WithVersions(versions);
        }

        private async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataFromLocalSourceAsync(SourceRepository localRepository,
            string packageId,
            CancellationToken token)
        {
            var localResource = await localRepository.GetResourceAsync<PackageMetadataResource>(token);

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var localPackages = await localResource?.GetMetadataAsync(
                    packageId,
                    includePrerelease: true,
                    includeUnlisted: true,
                    sourceCacheContext: sourceCacheContext,
                    log: NuGetLogger,
                    token: token);

                return localPackages;
            }
        }
    }
}
