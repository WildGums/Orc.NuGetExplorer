namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
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
    using Resolver = Orc.NuGetExplorer.Resolver;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Management;
    using NuGetExplorer.Management.Exceptions;
    using Orc.NuGetExplorer.Watchers;
    using System.IO.Packaging;
    using System.Runtime.CompilerServices;

    internal class PackageInstallationService : IPackageInstallationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILogger _nugetLogger;
        private readonly IFrameworkNameProvider _frameworkNameProvider;
        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;
        private readonly INuGetProjectConfigurationProvider _nuGetProjectConfigurationProvider;
        private readonly INuGetProjectContextProvider _nuGetProjectContextProvider;
        private readonly IFileDirectoryService _fileDirectoryService;
        private readonly IApiPackageRegistry _apiPackageRegistry;
        private readonly INuGetCacheManager _nuGetCacheManager;

        private readonly IDownloadingProgressTrackerService _downloadingProgressTrackerService;


        public PackageInstallationService(IFrameworkNameProvider frameworkNameProvider,
            ISourceRepositoryProvider sourceRepositoryProvider,
            INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider,
            INuGetProjectContextProvider nuGetProjectContextProvider,
            IFileDirectoryService fileDirectoryService,
            IApiPackageRegistry apiPackageRegistry,
            ILogger logger)
        {
            Argument.IsNotNull(() => frameworkNameProvider);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => nuGetProjectConfigurationProvider);
            Argument.IsNotNull(() => nuGetProjectContextProvider);
            Argument.IsNotNull(() => fileDirectoryService);
            Argument.IsNotNull(() => apiPackageRegistry);
            Argument.IsNotNull(() => logger);

            _frameworkNameProvider = frameworkNameProvider;
            _sourceRepositoryProvider = sourceRepositoryProvider;
            _nuGetProjectConfigurationProvider = nuGetProjectConfigurationProvider;
            _nuGetProjectContextProvider = nuGetProjectContextProvider;
            _fileDirectoryService = fileDirectoryService;
            _apiPackageRegistry = apiPackageRegistry;
            _nugetLogger = logger;

            _nuGetCacheManager = new NuGetCacheManager(_fileDirectoryService);

            var serviceLocator = ServiceLocator.Default;
            if (serviceLocator.IsTypeRegistered<IDownloadingProgressTrackerService>())
            {
                _downloadingProgressTrackerService = serviceLocator.ResolveType<IDownloadingProgressTrackerService>();
            }
        }

        public VersionFolderPathResolver InstallerPathResolver => new VersionFolderPathResolver(_fileDirectoryService.GetGlobalPackagesFolder());

        public async Task UninstallAsync(PackageIdentity package, IExtensibleProject project, IEnumerable<PackageReference> installedPackageReferences,
            CancellationToken cancellationToken = default)
        {
            List<string> failedEntries = null;
            ICollection<PackageIdentity> uninstalledPackages;

            var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);
            var projectConfig = _nuGetProjectConfigurationProvider.GetProjectConfig(project);
            var uninstallationContext = new UninstallationContext(false, false);

            _nugetLogger.LogInformation($"Uninstall package {package}, Target framework: {targetFramework}");

            if (projectConfig == null)
            {
                _nugetLogger.LogWarning("Current project doesn't implement any configuration for own packages");
            }

            using (var cacheContext = new SourceCacheContext())  // _nuGetCacheManager.GetCacheContext())
            {
                var dependencyInfoResource = await project.AsSourceRepository(_sourceRepositoryProvider)
                    .GetResourceAsync<DependencyInfoResource>(cancellationToken);

                var dependencyInfoResourceCollection = new DependencyInfoResourceCollection(dependencyInfoResource);

                var resolverContext = await ResolveDependenciesAsync(package, targetFramework, PackageIdentity.Comparer, dependencyInfoResourceCollection, cacheContext, true, cancellationToken);

                var packageReferences = installedPackageReferences.ToList();

                if (uninstallationContext.RemoveDependencies)
                {
                    uninstalledPackages = await GetPackagesCanBeUninstalled(resolverContext.AvailablePackages, packageReferences.Select(x => x.PackageIdentity));
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
                    var folderProject = new FolderNuGetProject(project.ContentPath);

                    if (folderProject.PackageExists(removedPackage))
                    {
                        _fileDirectoryService.DeleteDirectoryTree(folderProject.GetInstalledPath(removedPackage), out failedEntries);
                    }

                    if (projectConfig == null)
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

        public async Task<InstallerResult> InstallAsync(
            PackageIdentity package,
            IExtensibleProject project,
            IReadOnlyList<SourceRepository> repositories,
            bool ignoreMissingPackages = false,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1. Decide what framework version to use on package resolving

                var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

                _nugetLogger.LogInformation($"Installing package {package}, Target framework: {targetFramework}");

                // Check is this context needed
                //var resContext = new NuGet.PackageManagement.ResolutionContext();

                // Step 2. Build list of dependencies and determine DependencyBehavior if some packages are misssed in current feed
                Resolver.PackageResolverContext resolverContext = null;

                using (var cacheContext = new SourceCacheContext())
                {
                    var getDependencyResourcesTasks = repositories.Select(repo => repo.GetResourceAsync<DependencyInfoResource>());

                    var dependencyResources = (await getDependencyResourcesTasks.WhenAllOrException()).Where(x => x.IsSuccess && x.Result != null)
                        .Select(x => x.Result).ToArray();

                    var dependencyInfoResources = new DependencyInfoResourceCollection(dependencyResources);

                    resolverContext = await ResolveDependenciesAsync(package, targetFramework, PackageIdentityComparer.Default, dependencyInfoResources, cacheContext, ignoreMissingPackages, cancellationToken);
                }

                if (!resolverContext?.AvailablePackages?.Any() ?? false)
                {
                    var errorMessage = $"Package {package} cannot be resolved with current settings for chosen destination";
                    _nugetLogger.LogError(errorMessage);
                    return new InstallerResult(errorMessage);
                }

                using (var cacheContext = new SourceCacheContext())
                {
                    // Step 3. Try to check is main package can be downloaded from resource
                    var mainPackageInfo = resolverContext.AvailablePackages.FirstOrDefault(p => p.Id == package.Id);

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
                        throw new IncompatiblePackageException($"Package {package} incompatible with project target platform {targetFramework}");
                    }

                    // Step 5. Build install list using NuGet Resolver and select available resources
                    var resolver = new Resolver.PackageResolver();
                    var packagesInstallationList = resolver.Resolve(resolverContext, cancellationToken);

                    var availablePackagesToInstall = packagesInstallationList
                        .Select(
                            x => resolverContext.AvailablePackages
                                .Single(p => PackageIdentityComparer.Default.Equals(p, x)));

                    // Step 6. Download and extract all
                    _nugetLogger.LogInformation($"Downloading package dependencies...");
                    var downloadResults = await DownloadPackagesResourcesAsync(availablePackagesToInstall, cacheContext, cancellationToken);
                    _nugetLogger.LogInformation($"{downloadResults.Count - 1} dependencies downloaded");
                    var extractionContext = GetExtractionContext();
                    await ExtractPackagesResourcesAsync(downloadResults, project, extractionContext, cancellationToken);
                    await CheckLibAndFrameworkItemsAsync(downloadResults, targetFramework, cancellationToken);

                    return new InstallerResult(downloadResults);
                }
            }
            catch (NuGetResolverInputException ex)
            {
                throw new IncompatiblePackageException($"Package {package} or some of it dependencies are missed for current target framework", ex);
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
            var registrationResource = await sourceRepository.GetResourceAsync<RegistrationResourceV3>();
            var httpSourceResource = await sourceRepository.GetResourceAsync<HttpSourceResource>();
            var rawPackageMetadata = await registrationResource.GetPackageMetadata(packageIdentity, new SourceCacheContext(), _nugetLogger, default);

            if (rawPackageMetadata is null)
            {
                return null;
            }

            var catalogUrl = rawPackageMetadata.GetValue<string>("@id");
            var rawCatalogItem = await httpSourceResource.HttpSource.GetJObjectAsync(new HttpSourceRequest(catalogUrl, _nugetLogger), _nugetLogger, default);

            return rawCatalogItem?.GetValue<long>("packageSize");
        }

        private async Task<Resolver.PackageResolverContext> ResolveDependenciesAsync(PackageIdentity identity, NuGetFramework targetFramework, IEqualityComparer<PackageIdentity> equalityComparer,
            DependencyInfoResourceCollection dependencyInfoResource, SourceCacheContext cacheContext, bool ignoreMissingPackages = false, CancellationToken cancellationToken = default)
        {
            HashSet<SourcePackageDependencyInfo> packageStore = new HashSet<SourcePackageDependencyInfo>(equalityComparer);
            HashSet<PackageIdentity> ignoredPackages = new HashSet<PackageIdentity>();
            Stack<SourcePackageDependencyInfo> downloadStack = new Stack<SourcePackageDependencyInfo>();
            var resolvingBehavior = DependencyBehavior.Lowest;

            // get top dependency
            var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                            identity, targetFramework, cacheContext, _nugetLogger, cancellationToken);

            if (dependencyInfo == null)
            {
                _nugetLogger.LogError($"Cannot resolve {identity} package for target framework {targetFramework}");
                return Resolver.PackageResolverContext.Empty;
            }

            downloadStack.Push(dependencyInfo); //and add it to package store

            while (downloadStack.Count > 0)
            {
                var topPackage = downloadStack.Pop();

                if (!packageStore.Contains(topPackage))
                {
                    packageStore.Add(topPackage);
                }
                else
                {
                    continue;
                }

                foreach (var dependency in topPackage.Dependencies)
                {
                    // currently we use specific version during child dependency resolving 
                    // but possibly it should be configured in project
                    var dependencyIdentity = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);

                    var relatedDepInfo = await dependencyInfoResource.ResolvePackage(dependencyIdentity, targetFramework, cacheContext, _nugetLogger, cancellationToken);

                    if (relatedDepInfo != null)
                    {
                        downloadStack.Push(relatedDepInfo);
                        continue;
                    }

                    if (ignoreMissingPackages)
                    {
                        if (_apiPackageRegistry.IsRegistered(dependencyIdentity.Id))
                        {
                            resolvingBehavior = DependencyBehavior.Lowest;

                            if (!packageStore.Contains(dependencyIdentity))
                            {
                                packageStore.Add(new SourcePackageDependencyInfo(dependencyIdentity.Id, dependencyIdentity.Version, Enumerable.Empty<PackageDependency>(), false, null));
                            }

                            ignoredPackages.Add(dependencyIdentity);
                            await _nugetLogger.LogAsync(LogLevel.Information, $"The package dependency {dependencyIdentity.Id} listed as part of API and can be safely skipped");
                        }
                        else
                        {
                            resolvingBehavior = DependencyBehavior.Ignore;
                            await _nugetLogger.LogAsync(LogLevel.Warning, $"Available sources doesn't contain package {dependencyIdentity}. Package {dependencyIdentity} is missing");
                        }
                    }
                    else
                    {
                        throw new MissingPackageException($"Cannot find package {dependencyIdentity}");
                    }
                }
            }

            // Construct context for package resovler
            return GetResolverContext(identity, resolvingBehavior, packageStore, ignoredPackages);
        }

        private async Task<IDictionary<SourcePackageDependencyInfo, DownloadResourceResult>> DownloadPackagesResourcesAsync(
            IEnumerable<SourcePackageDependencyInfo> packageIdentities, SourceCacheContext cacheContext, CancellationToken cancellationToken)
        {
            var downloaded = new Dictionary<SourcePackageDependencyInfo, DownloadResourceResult>();

            string globalFolderPath = _fileDirectoryService.GetGlobalPackagesFolder();

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
            if (string.Equals(globalFolder, ""))
            {
                globalFolder = _fileDirectoryService.GetGlobalPackagesFolder();
            }

            using (var progressToken = await _downloadingProgressTrackerService?.TrackDownloadOperationAsync(this, package))
            {
                var downloadResource = await package.Source.GetResourceAsync<DownloadResource>(cancellationToken);

                var packageDownloadContext = new PackageDownloadContext(cacheContext);

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
        }

        private async Task ExtractPackagesResourcesAsync(
            IDictionary<SourcePackageDependencyInfo, DownloadResourceResult> packageResources,
            IExtensibleProject project,
            PackageExtractionContext extractionContext,
            CancellationToken cancellationToken)
        {
            List<PackageIdentity> extractedPackages = new List<PackageIdentity>();

            try
            {
                var pathResolver = new PackagePathResolver(project.ContentPath);

                foreach (var packageResource in packageResources)
                {
                    var downloadedPart = packageResource.Value;
                    var packageIdentity = packageResource.Key;

                    var nupkgPath = pathResolver.GetInstalledPackageFilePath(packageIdentity);

                    bool alreadyInstalled = File.Exists(nupkgPath);

                    try
                    {
                        _nugetLogger.LogInformation($"Extracting package {downloadedPart.GetResourceRoot()} to {project} project folder..");
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
                            throw new InvalidOperationException("An error occured during package extraction", ex);
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


        private Resolver.PackageResolverContext GetResolverContext(PackageIdentity package, DependencyBehavior dependencyBehavior, IEnumerable<SourcePackageDependencyInfo> availablePackages, IEnumerable<PackageIdentity> ignoredDependenciesList)
        {
            var idArray = new[] { package.Id };

            var resolverContext = new Resolver.PackageResolverContext(
                dependencyBehavior,
                idArray,
                requiredPackageIds: Enumerable.Empty<string>(),
                packagesConfig: Enumerable.Empty<PackageReference>(),
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
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => packageReader);

            var frameworkReducer = new FrameworkReducer();

            var libraries = await packageReader.GetLibItemsAsync(token);

            var bestMatches = frameworkReducer.GetNearest(targetFramework, libraries.Select(x => x.TargetFramework));

            return bestMatches != null;
        }

        private async Task<ICollection<PackageIdentity>> GetPackagesCanBeUninstalled(
            IEnumerable<SourcePackageDependencyInfo> markedForUninstall,
            IEnumerable<PackageIdentity> installedPackages)
        {
            IDictionary<PackageIdentity, HashSet<PackageIdentity>> dependenciesDictionary;
            var dependentsDictionary = UninstallResolver.GetPackageDependents(markedForUninstall, installedPackages, out dependenciesDictionary);

            //exclude packages which should not be removed, because of dependent packages
            HashSet<PackageIdentity> shouldBeExcludedSet = new HashSet<PackageIdentity>();

            foreach (var identity in dependenciesDictionary)
            {
                var markedOnUninstallDependency = identity.Key;

                HashSet<PackageIdentity> dependents;

                if (dependentsDictionary.TryGetValue(markedOnUninstallDependency, out dependents) && dependents != null)
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
                    var satelliteFiles = await GetSatelliteFilesForLibrary(libItemGroup, packageReader, cancellationToken);
                }
            }
        }

        private async Task<List<string>> GetSatelliteFilesForLibrary(FrameworkSpecificGroup libraryFrameworkSpecificGroup, PackageReaderBase packageReader,
            CancellationToken cancellationToken)
        {
            var satelliteFiles = new List<string>();

            var nuspec = await packageReader.GetNuspecAsync(cancellationToken);
            var nuspecReader = new NuspecReader(nuspec);

            var satelliteFilesInGroup = libraryFrameworkSpecificGroup.Items
            .Where(item =>
                Path.GetDirectoryName(item)
                    .Split(Path.DirectorySeparatorChar)
                    .Contains(nuspecReader.GetLanguage(), StringComparer.OrdinalIgnoreCase)).ToList();

            satelliteFiles.AddRange(satelliteFilesInGroup);

            return satelliteFiles;
        }
    }
}
