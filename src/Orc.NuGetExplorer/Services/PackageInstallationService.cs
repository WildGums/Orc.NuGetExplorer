namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Logging;
    using MethodTimer;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.PackageManagement;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Packaging.Signing;
    using NuGet.ProjectManagement;
    using NuGet.ProjectModel;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Management;
    using NuGetExplorer.Management.Exceptions;
    using Orc.FileSystem;
    using Resolver = Orc.NuGetExplorer.Resolver;

    internal class PackageInstallationService : IPackageInstallationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly VersionFolderPathResolver _installerPathResolver;

        private readonly ILogger _nugetLogger;
        private readonly IFrameworkNameProvider _frameworkNameProvider;
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;
        private readonly INuGetProjectConfigurationProvider _nuGetProjectConfigurationProvider;
        private readonly INuGetProjectContextProvider _nuGetProjectContextProvider;
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;
        private readonly IApiPackageRegistry _apiPackageRegistry;
        private readonly IFileSystemService _fileSystemService;
#pragma warning disable IDISP006 // Implement IDisposable.
        private readonly INuGetCacheManager _nuGetCacheManager;
#pragma warning restore IDISP006 // Implement IDisposable.
        private readonly IDownloadingProgressTrackerService _downloadingProgressTrackerService;


        public PackageInstallationService(IServiceLocator serviceLocator,
                                          IFrameworkNameProvider frameworkNameProvider,
                                          ISourceRepositoryProvider sourceRepositoryProvider,
                                          INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider,
                                          INuGetProjectContextProvider nuGetProjectContextProvider,
                                          IDirectoryService directoryService,
                                          IFileService fileService,
                                          IApiPackageRegistry apiPackageRegistry,
                                          IFileSystemService fileSystemService,
                                          IDownloadingProgressTrackerService downloadingProgressTrackerService,
                                          ILogger logger)
        {
            ArgumentNullException.ThrowIfNull(serviceLocator);
            ArgumentNullException.ThrowIfNull(frameworkNameProvider);
            ArgumentNullException.ThrowIfNull(sourceRepositoryProvider);
            ArgumentNullException.ThrowIfNull(nuGetProjectConfigurationProvider);
            ArgumentNullException.ThrowIfNull(nuGetProjectContextProvider);
            ArgumentNullException.ThrowIfNull(directoryService);
            ArgumentNullException.ThrowIfNull(fileService);
            ArgumentNullException.ThrowIfNull(apiPackageRegistry);
            ArgumentNullException.ThrowIfNull(fileSystemService);
            ArgumentNullException.ThrowIfNull(downloadingProgressTrackerService);
            ArgumentNullException.ThrowIfNull(logger);

            _frameworkNameProvider = frameworkNameProvider;
            _sourceRepositoryProvider = sourceRepositoryProvider;
            _nuGetProjectConfigurationProvider = nuGetProjectConfigurationProvider;
            _nuGetProjectContextProvider = nuGetProjectContextProvider;
            _directoryService = directoryService;
            _fileService = fileService;
            _apiPackageRegistry = apiPackageRegistry;
            _fileSystemService = fileSystemService;
            _downloadingProgressTrackerService = downloadingProgressTrackerService;
            _nugetLogger = logger;

            _nuGetCacheManager = new NuGetCacheManager(_directoryService, _fileService);

            _installerPathResolver = new VersionFolderPathResolver(DefaultNuGetFolders.GetGlobalPackagesFolder());
        }

        public VersionFolderPathResolver InstallerPathResolver => _installerPathResolver;

        public async Task UninstallAsync(PackageIdentity package, IExtensibleProject project, IEnumerable<PackageReference> installedPackageReferences,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(project);

            var failedEntries = new List<string>();
            ICollection<PackageIdentity> uninstalledPackages;

            var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

#if NET5_0_OR_GREATER
            var reducedFramework = new FrameworkReducer().ReduceUpwards(project.SupportedPlatforms).First();
            targetFramework = reducedFramework;
#endif

            var projectConfig = _nuGetProjectConfigurationProvider.GetProjectConfig(project);
            var uninstallationContext = new UninstallationContext(false, false);

            _nugetLogger.LogInformation($"Uninstall package {package}, Target framework: {targetFramework}");

            if (projectConfig is null)
            {
                _nugetLogger.LogWarning($"Project {project.Name} doesn't implement any configuration for own packages");
            }

            using (var cacheContext = new SourceCacheContext()
            {
                NoCache = false,
                DirectDownload = false,
            })
            {
                var dependencyInfoResource = await project.AsSourceRepository(_sourceRepositoryProvider)
                    .GetResourceAsync<DependencyInfoResource>(cancellationToken);

                var dependencyInfoResourceCollection = new DependencyInfoResourceCollection(dependencyInfoResource);

                var resolverContext = await ResolveDependenciesAsync(package, targetFramework, PackageIdentity.Comparer, dependencyInfoResourceCollection, cacheContext, project, true, cancellationToken);

                var packageReferences = installedPackageReferences.ToList();

                if (uninstallationContext.RemoveDependencies)
                {
                    uninstalledPackages = await GetPackagesCanBeUninstalledAsync(resolverContext.AvailablePackages, packageReferences.Select(x => x.PackageIdentity));
                }
                else
                {
                    uninstalledPackages = new List<PackageIdentity>() { package };
                }
            }

            try
            {
                foreach (var removedPackage in uninstalledPackages)
                {
                    if (removedPackage.Version is null)
                    {
                        _nugetLogger.LogWarning($"Skip package {removedPackage.Id} uninstall. Check your package.config for references of this packages");
                        continue;
                    }

                    var folderProject = new FolderNuGetProject(project.ContentPath);

                    if (folderProject.PackageExists(removedPackage))
                    {
                        _directoryService.ForceDeleteDirectory(_fileService, folderProject.GetInstalledPath(removedPackage), out failedEntries);
                    }

                    if (projectConfig is null)
                    {
                        continue;
                    }

                    var result = await projectConfig.UninstallPackageAsync(removedPackage, _nuGetProjectContextProvider.GetProjectContext(FileConflictAction.PromptUser), cancellationToken);
                    if (!result)
                    {
                        _nugetLogger.LogError($"Saving package configuration failed in project {project} when installing package {package}");
                    }

                }
            }
            catch (IOException ex)
            {
                Log.Error(ex);
                _nugetLogger.LogError("Package files cannot be complete deleted by unexpected error (may be directory in use by another process?");
            }
            finally
            {
                LogHelper.LogUnclearedPaths(failedEntries, Log);
            }
        }

        [Time]
        public async Task<InstallerResult> InstallAsync(
            PackageIdentity package,
            IExtensibleProject project,
            IReadOnlyList<SourceRepository> repositories,
            bool ignoreMissingPackages = false,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(repositories);

            try
            {
                // Step 1. Decide what framework version used on package resolving
                // Enforce platform-specific framework for .NET 5.0

                var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

#if NET5_0_OR_GREATER
                var mostSpecific = new FrameworkReducer().ReduceUpwards(project.SupportedPlatforms).First();
                targetFramework = mostSpecific;
#endif

                _nugetLogger.LogInformation($"Installing package {package}, Target framework: {targetFramework}");

                // Prepare to step 2. Add globals if cache enabled as available repository with highest priority.
                // Note: This part falls under responsibility of RepositoryContextService but the same logic used to determine what packages are found by IPackageLoaderService
                // To not break behavior for now add here
                if (!project.NoCache)
                {
                    var repositoryList = repositories.ToList();
                    repositoryList.Insert(0, new SourceRepository(new PackageSource(DefaultNuGetFolders.GetGlobalPackagesFolder(), ".nuget"), Repository.Provider.GetCoreV3()));
                    repositories = repositoryList;
                }

                // Step 2. Build list of dependencies and determine DependencyBehavior if some packages are misssed in current feed
                Resolver.PackageResolverContext? resolverContext = null;

                using (var cacheContext = new SourceCacheContext())
                {
#pragma warning disable IDISP013 // Await in using.
                    var getDependencyResourcesTasks = repositories.Select(repo => repo.GetResourceAsync<DependencyInfoResource>());
#pragma warning restore IDISP013 // Await in using.

                    var dependencyResources = (await getDependencyResourcesTasks.WhenAllOrExceptionAsync()).Where(x => x.IsSuccess && x.Result is not null)
                        .Select(x => x.Result!).ToArray();

                    var dependencyInfoResources = new DependencyInfoResourceCollection(dependencyResources);

                    resolverContext = await ResolveDependenciesAsync(package, targetFramework, PackageIdentityComparer.Default, dependencyInfoResources, cacheContext, project, ignoreMissingPackages, cancellationToken);

                    if (resolverContext is null ||
                        !(resolverContext?.AvailablePackages?.Any() ?? false))
                    {
                        var errorMessage = $"Package {package} cannot be resolved with current settings (TFM: {targetFramework}) for chosen destination";
                        _nugetLogger.LogWarning(errorMessage);
                        return new InstallerResult(errorMessage);
                    }

                    // Step 3. Try to check is main package can be downloaded from resource
                    var mainPackageInfo = resolverContext.AvailablePackages.First(p => p.Id == package.Id);

                    _nugetLogger.LogInformation($"Downloading {package}...");
                    var mainDownloadedFiles = await DownloadPackageResourceAsync(mainPackageInfo, cacheContext, cancellationToken);

                    _nugetLogger.LogInformation($"{package} download completed");

                    if (!mainDownloadedFiles.IsAvailable())
                    {
                        // Downlod failed by some reasons (probably connection issue or package goes deleted before feed updated)
                        var errorMessage = $"Current source lists package {package} but attempts to download it have failed. The source in invalid or required packages were removed while the current operation was in progress";
                        _nugetLogger.LogError(errorMessage);
                        return new InstallerResult(errorMessage);
                    }

                    // Step 4. Check is main package compatible with target Framework
                    var canBeInstalled = await CheckCanBeInstalledAsync(project, mainDownloadedFiles.PackageReader, targetFramework, cancellationToken);

                    if (!canBeInstalled)
                    {
                        throw Log.ErrorAndCreateException<IncompatiblePackageException>($"Package {package} incompatible with project target platform {targetFramework}");
                    }

                    // Step 5. Build install list using NuGet Resolver and select available resources. 
                    // Track packages which already installed and make sure only one version of package exists
                    var resolver = new Resolver.PackageResolver();
                    var availablePackagesToInstall = await resolver.ResolveWithVersionOverrideAsync(resolverContext, project, DependencyBehavior.Highest,
                        (project, conflict) => _fileSystemService.CreateDeleteme(conflict.PackageIdentity.Id, project.GetInstallPath(conflict.PackageIdentity)),
                        cancellationToken);

                    // Step 6. Download everything except main package and extract all
                    availablePackagesToInstall.Remove(mainPackageInfo);
                    _nugetLogger.LogInformation($"Downloading package dependencies...");
                    var downloadResults = await DownloadPackagesResourcesAsync(availablePackagesToInstall, cacheContext, cancellationToken);
                    downloadResults[mainPackageInfo] = mainDownloadedFiles;
                    _nugetLogger.LogInformation($"{downloadResults.Count - 1} dependencies downloaded");
                    var extractionContext = GetExtractionContext();
                    await ExtractPackagesResourcesAsync(downloadResults, project, extractionContext, cancellationToken);
                    await CheckLibAndFrameworkItemsAsync(downloadResults, targetFramework, cancellationToken);

                    return new InstallerResult(downloadResults);
                }
            }
            catch (NuGetResolverInputException ex)
            {
                throw Log.ErrorAndCreateException<IncompatiblePackageException>($"Package {package} or some of it dependencies are missed for current target framework", ex);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                throw;
            }
        }

        // TODO move to separate class
        public async Task<long?> MeasurePackageSizeFromRepositoryAsync(PackageIdentity packageIdentity, SourceRepository sourceRepository)
        {
            ArgumentNullException.ThrowIfNull(packageIdentity);
            ArgumentNullException.ThrowIfNull(sourceRepository);

            var registrationResource = await sourceRepository.GetResourceAsync<RegistrationResourceV3>();
            if (registrationResource is null)
            {
                return null;
            }

            var httpSourceResource = await sourceRepository.GetResourceAsync<HttpSourceResource>();
            if (httpSourceResource is null)
            {
                return null;
            }

            using (var sourceCacheContext = new SourceCacheContext())
            {
                var rawPackageMetadata = await registrationResource.GetPackageMetadata(packageIdentity, sourceCacheContext, _nugetLogger, default);
                if (rawPackageMetadata is null)
                {
                    return null;
                }

                var catalogUrl = rawPackageMetadata.GetValue<string>("@id");
                var rawCatalogItem = await httpSourceResource.HttpSource.GetJObjectAsync(new HttpSourceRequest(catalogUrl, _nugetLogger), _nugetLogger, default);

                return rawCatalogItem?.GetValue<long>("packageSize");
            }
        }

        private async Task<Resolver.PackageResolverContext> ResolveDependenciesAsync(PackageIdentity identity, NuGetFramework targetFramework, IEqualityComparer<PackageIdentity> equalityComparer,
            DependencyInfoResourceCollection dependencyInfoResource, SourceCacheContext cacheContext, IExtensibleProject project, bool ignoreMissingPackages = false, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(identity);
            ArgumentNullException.ThrowIfNull(targetFramework);
            ArgumentNullException.ThrowIfNull(equalityComparer);
            ArgumentNullException.ThrowIfNull(dependencyInfoResource);
            ArgumentNullException.ThrowIfNull(project);

            // The collection of already processed packages
            var packageStore = new HashSet<SourcePackageDependencyInfo>(equalityComparer);
            var ignoredPackages = new HashSet<PackageIdentity>();
            var downloadStack = new Stack<SourcePackageDependencyInfo>();
            var resolvingBehavior = DependencyBehavior.Lowest;

            // get top dependency
            var dependencyInfo = await dependencyInfoResource.ResolvePackageAsync(
                            identity, targetFramework, cacheContext, _nugetLogger, cancellationToken);

            if (dependencyInfo is null)
            {
                _nugetLogger.LogError($"Cannot resolve {identity} package for target framework {targetFramework}");
                return Resolver.PackageResolverContext.Empty;
            }

            downloadStack.Push(dependencyInfo); //and add it to package store

            while (downloadStack.Count > 0)
            {
                var nextPackage = downloadStack.Pop();

                if (packageStore.Contains(nextPackage))
                {
                    continue;
                }
                else
                {
                    packageStore.Add(nextPackage);
                }

                foreach (var dependency in nextPackage.Dependencies)
                {
                    // currently we use specific version during child dependency resolving 
                    // but possibly it should be configured in project
                    var dependencyIdentity = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);

                    var isPackageRequiresOwnDependencies = !_apiPackageRegistry.IsRegistered(dependencyIdentity.Id);
                    if (isPackageRequiresOwnDependencies)
                    {
                        // We can't determine the unknown version yet from range, but can exclude min required if it was already processed
                        if (packageStore.Contains(dependencyIdentity))
                        {
                            continue;
                        }

                        var relatedDepInfos = await dependencyInfoResource.ResolvePackagesWithVersionSatisfyRangeAsync(dependencyIdentity, dependency.VersionRange, targetFramework, cacheContext, _nugetLogger, cancellationToken);
                        foreach (var relatedDepedencyInfoResource in relatedDepInfos)
                        {
                            downloadStack.Push(relatedDepedencyInfoResource);
                        }

                        if (relatedDepInfos.Any())
                        {
                            // we found compatible (at least with target framework) packages and leave decision to Package Resolver in the future
                            continue;
                        }
                    }

                    // Determine behavior if package cannot be resolved in any way
                    if (ignoreMissingPackages)
                    {
                        if (_apiPackageRegistry.IsRegistered(dependencyIdentity.Id))
                        {
                            resolvingBehavior = DependencyBehavior.Lowest;

                            if (!packageStore.Contains(dependencyIdentity))
                            {
                                packageStore.Add(new SourcePackageDependencyInfo(dependencyIdentity.Id, dependencyIdentity.Version, Enumerable.Empty<PackageDependency>(), false, null));
                            }

                            if (ignoredPackages.Add(dependencyIdentity))
                            {
                                // Show only for top package, not much effort to see this message multiple times
                                if (nextPackage == dependencyInfo)
                                {
                                    await _nugetLogger.LogAsync(LogLevel.Information, $"Package dependency {dependencyIdentity.Id} listed as part of API and can be safely skipped");
                                }
                            }
                        }
                        else
                        {
                            resolvingBehavior = DependencyBehavior.Ignore;
                            await _nugetLogger.LogAsync(LogLevel.Warning, $"Available sources doesn't contain package {dependencyIdentity}. Package {dependencyIdentity} is missing");
                        }
                    }
                    else
                    {
                        throw Log.ErrorAndCreateException<MissingPackageException>($"Cannot find package {dependencyIdentity}");
                    }
                }
            }


            // Pass packages.config to resolver
            var nugetPackagesConfigProject = _nuGetProjectConfigurationProvider.GetProjectConfig(project);
            var packagesConfigReferences = await nugetPackagesConfigProject.GetInstalledPackagesAsync(cancellationToken);

            // Construct context for package resolver
            return GetResolverContext(identity, resolvingBehavior, packageStore, packagesConfigReferences, ignoredPackages);
        }

        private async Task<IDictionary<SourcePackageDependencyInfo, DownloadResourceResult>> DownloadPackagesResourcesAsync(
            IEnumerable<SourcePackageDependencyInfo> packageIdentities, SourceCacheContext cacheContext, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(packageIdentities);

            var downloaded = new Dictionary<SourcePackageDependencyInfo, DownloadResourceResult>();

            var globalFolderPath = DefaultNuGetFolders.GetGlobalPackagesFolder();

            foreach (var package in packageIdentities)
            {
                var downloadResult = await DownloadPackageResourceAsync(package, cacheContext, cancellationToken, globalFolderPath);

                downloaded.Add(package, downloadResult);
            }

            return downloaded;
        }

        private async Task<DownloadResourceResult> DownloadPackageResourceAsync(
            SourcePackageDependencyInfo package, SourceCacheContext cacheContext, CancellationToken cancellationToken, string globalFolder = "")
        {
            ArgumentNullException.ThrowIfNull(package);

            if (string.Equals(globalFolder, string.Empty))
            {
                globalFolder = DefaultNuGetFolders.GetGlobalPackagesFolder();
            }

            using var progressToken = await _downloadingProgressTrackerService.TrackDownloadOperationAsync(this, package);
            var downloadResource = await package.Source.GetResourceAsync<DownloadResource>(cancellationToken);

            var downloadResult = await downloadResource.GetDownloadResourceResultAsync
            (
                package,
                new PackageDownloadContext(cacheContext),
                globalFolder,
                _nugetLogger,
                cancellationToken
            );

            return downloadResult;
        }

        private async Task ExtractPackagesResourcesAsync(
            IDictionary<SourcePackageDependencyInfo, DownloadResourceResult> packageResources,
            IExtensibleProject project,
            PackageExtractionContext extractionContext,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(packageResources);
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(extractionContext);

            var extractedPackages = new List<PackageIdentity>();

            try
            {
                var pathResolver = project.GetPathResolver() ?? new PackagePathResolver(project.ContentPath);

                foreach (var packageResource in packageResources)
                {
                    var downloadedPart = packageResource.Value;
                    var packageIdentity = packageResource.Key;

                    var nupkgPath = pathResolver.GetInstalledPackageFilePath(packageIdentity);
                    var alreadyInstalled = string.IsNullOrEmpty(nupkgPath) ? false : _fileService.Exists(nupkgPath);

                    try
                    {
                        _nugetLogger.LogInformation($"Extracting package {downloadedPart.GetResourceRoot()} to {project} project folder");

                        var extractedPaths = await PackageExtractor.ExtractPackageAsync(
                                downloadedPart.PackageSource,
                                downloadedPart.PackageStream,
                                pathResolver,
                                extractionContext,
                                cancellationToken
                        );

                        _nugetLogger.LogInformation($"Successfully unpacked {extractedPaths.Count()} files");

                        if (!alreadyInstalled)
                        {
                            extractedPackages.Add(packageIdentity);
                        }
                    }
                    catch (IOException ex)
                    {
                        if (alreadyInstalled)
                        {
                            // supress error
                            _nugetLogger.LogInformation($"Package {packageIdentity} already located in extraction directory");

                            // TODO: verify installation?
                        }
                        else
                        {
                            throw Log.ErrorAndCreateException<InvalidOperationException>("An error occured during package extraction", ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);

                var extractionException = new ProjectInstallException(ex.Message, ex)
                {
                    CurrentBatch = extractedPackages
                };

                throw extractionException;
            }
        }


        private Resolver.PackageResolverContext GetResolverContext(PackageIdentity package, DependencyBehavior dependencyBehavior, IEnumerable<SourcePackageDependencyInfo> availablePackages,
            IEnumerable<PackageReference> packagesConfig, IEnumerable<PackageIdentity> ignoredDependenciesList)
        {
            ArgumentNullException.ThrowIfNull(package);
            ArgumentNullException.ThrowIfNull(availablePackages);

            var idArray = new[] { package.Id };

            var requiredPackageIds = availablePackages.Select(x => x.Id).Distinct();

            var resolverContext = new Resolver.PackageResolverContext(
                dependencyBehavior,
                idArray,
                requiredPackageIds: requiredPackageIds,
                packagesConfig: packagesConfig,
                preferredVersions: Enumerable.Empty<PackageIdentity>(),
                availablePackages,
                _sourceRepositoryProvider.GetRepositories().Select(x => x.PackageSource),
                ignoredDependenciesList.Select(x => x.Id),
                _nugetLogger
            );

            return resolverContext;
        }

        private PackageExtractionContext GetExtractionContext()
        {
            // TODO read and check signatures
            var signaturesCerts = Enumerable.Empty<TrustedSignerAllowListEntry>().ToList();

            var policyContextForClient = ClientPolicyContext.GetClientPolicy(Settings.LoadDefaultSettings(null), _nugetLogger);

            var extractionContext = new PackageExtractionContext(
                PackageSaveMode.Defaultv3,
                XmlDocFileSaveMode.Skip,
                policyContextForClient,
                _nugetLogger
            );

            return extractionContext;
        }

        private async Task<bool> CheckCanBeInstalledAsync(IExtensibleProject project, PackageReaderBase packageReader, NuGetFramework targetFramework, CancellationToken token)
        {
            ArgumentNullException.ThrowIfNull(project);
            ArgumentNullException.ThrowIfNull(packageReader);
            ArgumentNullException.ThrowIfNull(targetFramework);

            var frameworkReducer = new FrameworkReducer();

            var libraries = await packageReader.GetLibItemsAsync(token);
            var libraryTfms = libraries.Select(x => x.TargetFramework).ToList();

            var bestMatches = frameworkReducer.GetNearest(targetFramework, libraryTfms);

            if (bestMatches is null)
            {
                // Try to find first supported platform-specific version
                foreach (var platformSpecific in project.SupportedPlatforms)
                {
                    bestMatches = frameworkReducer.GetNearest(platformSpecific, libraryTfms);

                    if (bestMatches is not null)
                    {
                        break;
                    }
                }
            }

            return bestMatches is not null;
        }

        private async Task<ICollection<PackageIdentity>> GetPackagesCanBeUninstalledAsync(
            IEnumerable<SourcePackageDependencyInfo> markedForUninstall,
            IEnumerable<PackageIdentity> installedPackages)
        {
            var dependentsDictionary = UninstallResolver.GetPackageDependents(markedForUninstall, installedPackages, out var dependenciesDictionary);

            //exclude packages which should not be removed, because of dependent packages
            var shouldBeExcludedSet = new HashSet<PackageIdentity>();

            foreach (var identity in dependenciesDictionary)
            {
                var markedOnUninstallDependency = identity.Key;


                if (dependentsDictionary.TryGetValue(markedOnUninstallDependency, out var dependents) && dependents is not null)
                {
                    var externalDependants = dependents.Where(x => !dependenciesDictionary.ContainsKey(x)).ToList();

                    if (externalDependants.Count > 0)
                    {
                        _nugetLogger.LogInformation($"{identity} package skipped, because one or more installed packages depends on it");
                    }

                    externalDependants.ForEach(d => shouldBeExcludedSet.Add(d));
                }
            }

            var markedForUninstallList = markedForUninstall.OfType<PackageIdentity>().ToList();

            foreach (var excludedDependency in shouldBeExcludedSet)
            {
                markedForUninstallList.Remove(excludedDependency);
            }

            return markedForUninstallList;
        }

        private async Task CheckLibAndFrameworkItemsAsync(IDictionary<SourcePackageDependencyInfo, DownloadResourceResult> downloadedPackagesDictionary,
            NuGetFramework targetFramework, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(downloadedPackagesDictionary);
            ArgumentNullException.ThrowIfNull(targetFramework);

            var frameworkReducer = new FrameworkReducer();

            foreach (var package in downloadedPackagesDictionary.Keys)
            {
                var packageReader = downloadedPackagesDictionary[package].PackageReader;

                var libraries = await packageReader.GetLibItemsAsync(cancellationToken);

                var bestMatches = frameworkReducer.GetNearest(targetFramework, libraries.Select(x => x.TargetFramework));

                var frameworkItems = await packageReader.GetFrameworkItemsAsync(cancellationToken);

                var nearestFrameworkItems = frameworkReducer.GetNearest(targetFramework, frameworkItems.Select(x => x.TargetFramework));

                foreach (var libItemGroup in libraries)
                {
                    var satelliteFiles = await GetSatelliteFilesForLibraryAsync(libItemGroup, packageReader, cancellationToken);
                }
            }
        }

        private async Task<List<string>> GetSatelliteFilesForLibraryAsync(FrameworkSpecificGroup libraryFrameworkSpecificGroup, PackageReaderBase packageReader,
            CancellationToken cancellationToken)
        {
            var satelliteFiles = new List<string>();

            using (var nuspec = await packageReader.GetNuspecAsync(cancellationToken))
            {
                var nuspecReader = new NuspecReader(nuspec);
                var satelliteFilesInGroup = libraryFrameworkSpecificGroup.Items
                        .Where(item => Path.GetDirectoryName(item)?.Split(Path.DirectorySeparatorChar)?.Contains(nuspecReader.GetLanguage(), StringComparer.OrdinalIgnoreCase) ?? false)
                        .ToList();

                satelliteFiles.AddRange(satelliteFilesInGroup);

                return satelliteFiles;
            }
        }
    }
}
