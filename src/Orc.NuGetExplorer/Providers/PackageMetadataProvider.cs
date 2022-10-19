namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using Orc.FileSystem;
    using Orc.NuGetExplorer.Management;

    public class PackageMetadataProvider : IPackageMetadataProvider
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly ILogger NuGetLogger;
        private readonly IDirectoryService _directoryService;
        private readonly ISourceRepositoryProvider _repositoryProvider;
        private readonly IEnumerable<SourceRepository> _sourceRepositories;

        private readonly IEnumerable<SourceRepository> _optionalLocalRepositories;

        private readonly Lazy<IExtensibleProject> _project = new(() => ServiceLocator.Default.ResolveRequiredType<IDefaultExtensibleProjectProvider>().GetDefaultProject());

        private SourceRepository? _localRepository;

        static PackageMetadataProvider()
        {
            NuGetLogger = ServiceLocator.Default.ResolveRequiredType<ILogger>();
        }

        public PackageMetadataProvider(IDirectoryService directoryService, IRepositoryService repositoryService, ISourceRepositoryProvider repositoryProvider)
        {
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(repositoryService);
            ArgumentNullException.ThrowIfNull(repositoryProvider);

            _directoryService = directoryService;
            _sourceRepositories = repositoryProvider.GetRepositories();
            _optionalLocalRepositories = new[]
            {
                repositoryProvider.CreateRepository(repositoryService.LocalRepository.ToPackageSource())
            };
            _repositoryProvider = repositoryProvider;
        }

        public PackageMetadataProvider(IEnumerable<SourceRepository> sourceRepositories, IEnumerable<SourceRepository> optionalGlobalLocalRepositories,
            IDirectoryService directoryService, ISourceRepositoryProvider repositoryProvider)
        {
            ArgumentNullException.ThrowIfNull(sourceRepositories);
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(repositoryProvider);

            _sourceRepositories = sourceRepositories;
            _optionalLocalRepositories = optionalGlobalLocalRepositories;
            _directoryService = directoryService;
            _repositoryProvider = repositoryProvider;
        }

        public static PackageMetadataProvider CreateFromSourceContext(IServiceLocator serviceLocator)
        {
            var directoryService = serviceLocator.ResolveRequiredType<IDirectoryService>();
            var repositoryService = serviceLocator.ResolveRequiredType<IRepositoryContextService>();
            var projectSource = serviceLocator.ResolveRequiredType<IExtensibleProjectLocator>();
            var packageManager = serviceLocator.ResolveRequiredType<INuGetPackageManager>();

            return PackageMetadataProvider.CreateFromSourceContext(directoryService, repositoryService, projectSource, packageManager);
        }

        public static PackageMetadataProvider CreateFromSourceContext(IDirectoryService directoryService, IRepositoryContextService repositoryService, IExtensibleProjectLocator projectSource,
            INuGetPackageManager projectManager)
        {
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(repositoryService);
            ArgumentNullException.ThrowIfNull(projectSource);
            ArgumentNullException.ThrowIfNull(projectManager);

            var typeFactory = TypeFactory.Default;

            var context = repositoryService.AcquireContext();

            var projects = projectSource.GetAllExtensibleProjects();

            var localRepos = projectManager.AsLocalRepositories(projects);

            var repos = context.Repositories ?? context.PackageSources?.Select(src => repositoryService.GetRepository(src)) ?? new List<SourceRepository>();

            return typeFactory.CreateRequiredInstanceWithParametersAndAutoCompletion<PackageMetadataProvider>(repos, localRepos);
        }


        public async Task<IPackageSearchMetadata?> GetLocalPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(identity);

            var sources = new List<SourceRepository>();

            if (_optionalLocalRepositories is not null)
            {
                sources.AddRange(_optionalLocalRepositories);
            }

            // Support multiple destinations
            if (_localRepository is null)
            {
                var project = _project.Value;
                if (project is not null && project.SupportSideBySide)
                {
                    // Note: ADD UNIT TEST HERE, Important
                    var localProjectDirectory = Directory.GetParent(project.GetInstallPath(identity));
                    if (localProjectDirectory is null)
                    {
                        Log.Warning("Cannot find destination folder in Side-but-side installation. Check project installation path.");
                    }
                    else
                    {
                        _localRepository = _repositoryProvider.CreateRepository(new PackageSource(localProjectDirectory.FullName), NuGet.Protocol.FeedType.FileSystemV2);
                    }
                }
            }

            if (_localRepository is not null)
            {
                sources.Add(_localRepository);
            }

            // Take the package from the first source it is found in
            foreach (var source in sources)
            {
                var result = await GetPackageMetadataFromLocalSourceAsync(source, identity, cancellationToken);

                if (result is not null)
                {
                    //TODO why additional fetching needed?
                    //return result.WithVersions(
                    //    () => FetchAndMergeVersionsAsync(identity, includePrerelease, ))
                    return result;
                }
            }

            return null;
        }

        public async Task<IPackageSearchMetadata?> GetLowestLocalPackageMetadataAsync(string packageid, bool includePrrelease, CancellationToken cancellationToken)
        {
            var sources = new List<SourceRepository>();

            if (_optionalLocalRepositories is not null)
            {
                sources.AddRange(_optionalLocalRepositories);
            }

            var tasks = sources.Select(r => GetPackageMetadataFromLocalSourceAsync(r, packageid, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess)
                .Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);

            var lowest = completed.SelectMany(p => p!)
                .OrderBy(p => p.Identity.Version)
                .FirstOrDefault();

            return lowest;
        }


        public async Task<IPackageSearchMetadata?> GetPackageMetadataAsync(PackageIdentity identity, bool includePrerelease, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(identity);

            if (!_sourceRepositories.Any())
            {
                Log.Warning("No repositories available");
                return null;
            }

            var tasks = _sourceRepositories
               .Select(r => GetPackageMetadataAsyncFromSourceAsync(r, identity, includePrerelease, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess)
                .Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);


            var master = completed.FirstOrDefault()
                ?? completed.FirstOrDefault()
                ?? PackageSearchMetadataBuilder.FromIdentity(identity).Build();

            //return master.WithVersions(
            //    asyncValueFactory: () => MergeVersionsAsync(identity, completed));

            return master;
        }

        public async Task<IPackageSearchMetadata?> GetHighestPackageMetadataAsync(string packageId, bool includePrerelease, CancellationToken cancellationToken)
        {
            //returned type - packageRegistrationMetadata
            var metadataList = await GetPackageMetadataListAsync(packageId, includePrerelease, false, cancellationToken);

            var master = metadataList.OrderByDescending(x => x.Identity.Version).FirstOrDefault();

            return master?.WithVersions(() => metadataList.ToVersionInfo(includePrerelease));
        }

        public async Task<IPackageSearchMetadata?> GetHighestPackageMetadataAsync(string packageId, bool includePrerelease, string[] ignoredReleases, CancellationToken cancellationToken)
        {
            var metadataList = await GetPackageMetadataListAsync(packageId, includePrerelease, false, cancellationToken);

            var master = metadataList.OrderByDescending(x => x.Identity.Version).FirstOrDefault(x => !x.Identity.Version.Release.ContainsAny(ignoredReleases, StringComparison.OrdinalIgnoreCase));

            return master?.WithVersions(() => metadataList.ToVersionInfo(includePrerelease));
        }

        public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataListAsync(string packageId, bool includePrerelease, bool includeUnlisted, CancellationToken cancellationToken)
        {
            var tasks = _sourceRepositories.Select(repo => GetPackageMetadataListAsyncFromSourceAsync(repo, packageId, includePrerelease, includeUnlisted, cancellationToken)).ToArray();

            var completed = (await tasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess).
                Select(x => x.UnwrapResult())
                .Where(metadata => metadata is not null);

            var packages = completed.SelectMany(p => p!);

            var uniquePackages = packages
                .GroupBy(
                   m => m.Identity.Version,
                   (v, ms) => ms.First());

            return uniquePackages ?? Array.Empty<IPackageSearchMetadata>();
        }

        /// <summary>
        /// Returns list of package metadata objects from repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="packageId"></param>
        /// <param name="includePrerelease"></param>
        /// <param name="includeUnlisted"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataListAsyncFromSourceAsync(SourceRepository repository,
            string packageId,
            bool includePrerelease,
            bool includeUnlisted,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(repository);

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

                var packages = await metadataResource.GetMetadataAsync(
                    packageId,
                    includePrerelease,
                    includeUnlisted,
                    sourceCacheContext,
                    NuGetLogger,
                    cancellationToken) ?? Array.Empty<IPackageSearchMetadata>();

                Log.Debug($"Found packages metadata for package {packageId}, count: {packages.Count()}");

                return packages;

            }
        }

        /// <summary>
        /// Returns list of package metadata objects along with all version metadtas from repository
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="identity"></param>
        /// <param name="includePrerelease"></param>
        /// <param name="takeVersions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<IPackageSearchMetadata?> GetPackageMetadataAsyncFromSourceAsync(SourceRepository repository,
            PackageIdentity identity,
            bool includePrerelease,
            CancellationToken cancellationToken,
            bool takeVersions = true)
        {
            ArgumentNullException.ThrowIfNull(repository);
            ArgumentNullException.ThrowIfNull(identity);

            if (takeVersions)
            {
                //query all versions and pack them in a single object
                var versionsMetadatas = await GetPackageMetadataListAsyncFromSourceAsync(repository, identity.Id, includePrerelease, false, cancellationToken);
                if (versionsMetadatas is null)
                {
                    return null;
                }

                if (!versionsMetadatas.Any())
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

                var package = await metadataResource.GetMetadataAsync(identity, sourceCacheContext, NuGetLogger, cancellationToken);
                return package;
            }
        }


        private async Task<IPackageSearchMetadata?> GetPackageMetadataFromLocalSourceAsync(SourceRepository localRepository, PackageIdentity packageIdentity, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(localRepository);
            ArgumentNullException.ThrowIfNull(packageIdentity);

            var localPackages = await GetPackageMetadataFromLocalSourceAsync(localRepository, packageIdentity.Id, token);

            var packageMetadata = localPackages?.FirstOrDefault(p => p.Identity.Version == packageIdentity.Version);

            var versions = new[]
            {
                    new VersionInfo(packageIdentity.Version)
                };

            return packageMetadata?.WithVersions(versions);

        }

        private async Task<IEnumerable<IPackageSearchMetadata>> GetPackageMetadataFromLocalSourceAsync(
            SourceRepository localRepository,
            string packageId,
            CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(localRepository);

            var localResource = await localRepository.GetResourceAsync<PackageMetadataResource>(token);

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var localPackages = await localResource.GetMetadataAsync(
                    packageId,
                    includePrerelease: true,
                    includeUnlisted: true,
                    sourceCacheContext: sourceCacheContext,
                    log: NuGetLogger,
                    token: token) ?? Array.Empty<IPackageSearchMetadata>();

                return localPackages;
            }
        }
    }
}
