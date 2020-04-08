namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Frameworks;
    using NuGet.PackageManagement;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Packaging.Signing;
    using NuGet.ProjectManagement;
    using NuGet.Protocol.Core.Types;
    using NuGet.Resolver;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Management;
    using NuGetExplorer.Management.Exceptions;

    internal class PackageInstallationService : IPackageInstallationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ILogger _nugetLogger;

        private readonly IFrameworkNameProvider _frameworkNameProvider;

        private readonly ISourceRepositoryProvider _sourceRepositoryProvider;

        private readonly INuGetProjectConfigurationProvider _nuGetProjectConfigurationProvider;

        private readonly INuGetProjectContextProvider _nuGetProjectContextProvider;

        private readonly IFileDirectoryService _fileDirectoryService;

        private readonly INuGetCacheManager _nuGetCacheManager;


        public PackageInstallationService(IFrameworkNameProvider frameworkNameProvider,
            ISourceRepositoryProvider sourceRepositoryProvider,
            INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider,
            INuGetProjectContextProvider nuGetProjectContextProvider,
            IFileDirectoryService fileDirectoryService,
            ILogger logger)
        {
            Argument.IsNotNull(() => frameworkNameProvider);
            Argument.IsNotNull(() => sourceRepositoryProvider);
            Argument.IsNotNull(() => nuGetProjectConfigurationProvider);
            Argument.IsNotNull(() => nuGetProjectContextProvider);
            Argument.IsNotNull(() => fileDirectoryService);
            Argument.IsNotNull(() => logger);

            _frameworkNameProvider = frameworkNameProvider;
            _sourceRepositoryProvider = sourceRepositoryProvider;
            _nuGetProjectConfigurationProvider = nuGetProjectConfigurationProvider;
            _nuGetProjectContextProvider = nuGetProjectContextProvider;
            _fileDirectoryService = fileDirectoryService;
            _nugetLogger = logger;

            _nuGetCacheManager = new NuGetCacheManager(_fileDirectoryService);
        }

        public async Task UninstallAsync(PackageIdentity package, IExtensibleProject project, IEnumerable<PackageReference> installedPackageReferences,
            CancellationToken cancellationToken = default)
        {
            List<string> failedEntries = null;
            ICollection<PackageIdentity> uninstalledPackages;

            var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);
            var projectConfig = _nuGetProjectConfigurationProvider.GetProjectConfig(project);

            Log.Info($"Uninstall package {package}, Target framework: {targetFramework}");

            if (projectConfig == null)
            {
                Log.Warning("Current project does not implement configuration for own packages");
            }

            //gather all dependencies
            var installedDependencyInfos = new HashSet<SourcePackageDependencyInfo>(PackageIdentity.Comparer);

            using (var cacheContext = new SourceCacheContext())  // _nuGetCacheManager.GetCacheContext())
            {
                var dependencyInfoResource = await project.AsSourceRepository(_sourceRepositoryProvider)
                    .GetResourceAsync<DependencyInfoResource>(cancellationToken);

                var dependencyInfoResourceCollection = new DependencyInfoResourceCollection(dependencyInfoResource);

                await ResolveDependenciesRecursivelyAsync(package, targetFramework, dependencyInfoResourceCollection, cacheContext, installedDependencyInfos, true, cancellationToken);

                var packageReferences = installedPackageReferences.ToList();

                uninstalledPackages = await GetPackagesCanBeUninstalled(installedDependencyInfos, packageReferences.Select(x => x.PackageIdentity), null);
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
                        Log.Error($"Saving package configuration failed in project {project} when installing package {package}");
                    }
                }
            }
            catch (IOException ex)
            {
                Log.Error(ex, "Package files cannot be complete deleted by unexpected error (may be directory in use by another process?");
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
                var targetFramework = FrameworkParser.TryParseFrameworkName(project.Framework, _frameworkNameProvider);

                Log.Info($"Installing package {package}, Target framework: {targetFramework}");

                //todo check is this context needed
                //var resContext = new NuGet.PackageManagement.ResolutionContext();

                var availabePackageStorage = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                DependencyBehavior dependencyBehavior = DependencyBehavior.Lowest;

                using (var cacheContext = new SourceCacheContext())   //_nuGetCacheManager.GetCacheContext())
                {
                    var getDependencyResourcesTasks = repositories.Select(repo => repo.GetResourceAsync<DependencyInfoResource>());

                    var dependencyResources = (await getDependencyResourcesTasks.WhenAllOrException()).Where(x => x.IsSuccess && x.Result != null)
                        .Select(x => x.Result).ToArray();

                    var dependencyInfoResources = new DependencyInfoResourceCollection(dependencyResources);

                    dependencyBehavior = await ResolveDependenciesRecursivelyAsync(package, targetFramework, dependencyInfoResources, cacheContext,
                        availabePackageStorage, ignoreMissingPackages, cancellationToken);
                }

                if (!availabePackageStorage.Any())
                {
                    var errorMessage = $"Package {package} cannot be resolved with current settings for chosen destination";
                    Log.Error(errorMessage);
                    return new InstallerResult(errorMessage);
                }

                using (var cacheContext = new SourceCacheContext())  // _nuGetCacheManager.GetCacheContext())
                {
                    //select main sourceDependencyInfo
                    var mainPackageInfo = availabePackageStorage.FirstOrDefault(p => p.Id == package.Id);

                    //try to download main package and check it target version
                    var mainDownloadedFiles = await DownloadPackageResourceAsync(mainPackageInfo, cacheContext, cancellationToken);

                    if (!mainDownloadedFiles.IsAvailable())
                    {
                        //download failed, possible because of package goes deleted during operation or feed is valid only for searching
                        //or connection failed
                        var errorMessage = $"Current source lists package {package} but attempts to download it have failed. The source in invalid or required packages were removed while the current operation was in progress";
                        Log.Error(errorMessage);
                        return new InstallerResult(errorMessage);
                    }

                    var canBeInstalled = await CheckCanBeInstalledAsync(project, mainDownloadedFiles.PackageReader, targetFramework, cancellationToken);

                    if (!canBeInstalled)
                    {
                        throw new IncompatiblePackageException($"Package {package} incompatible with project target platform {targetFramework}");
                    }

                    var resolverContext = GetResolverContext(package, dependencyBehavior, availabePackageStorage);

                    var resolver = new PackageResolver();

                    var packagesInstallationList = resolver.Resolve(resolverContext, cancellationToken);

                    var availablePackagesToInstall = packagesInstallationList
                        .Select(
                            x => availabePackageStorage
                                .Single(p => PackageIdentityComparer.Default.Equals(p, x)));


                    //accure downloadResourceResults for all package identities
                    var downloadResults = await DownloadPackagesResourcesAsync(availablePackagesToInstall, cacheContext, cancellationToken);

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

        private async Task<DependencyBehavior> ResolveDependenciesRecursivelyAsync(PackageIdentity identity, NuGetFramework targetFramework,
            DependencyInfoResourceCollection dependencyInfoResource,
            SourceCacheContext cacheContext,
            HashSet<SourcePackageDependencyInfo> storage,
            bool ignoreMissingPackages = false,
            CancellationToken cancellationToken = default)
        {
            Argument.IsNotNull(() => storage);

            HashSet<SourcePackageDependencyInfo> packageStore = storage;

            Stack<SourcePackageDependencyInfo> downloadStack = new Stack<SourcePackageDependencyInfo>();

            var resolvedBehavior = DependencyBehavior.Lowest;

            //get top dependency
            var dependencyInfo = await dependencyInfoResource.ResolvePackage(
                            identity, targetFramework, cacheContext, _nugetLogger, cancellationToken);

            if (dependencyInfo == null)
            {
                Log.Error($"Cannot resolve {identity} package for target framework {targetFramework}");
                return resolvedBehavior;
            }

            downloadStack.Push(dependencyInfo); //and add it to package store

            while (downloadStack.Count > 0)
            {
                var rootDependency = downloadStack.Pop();

                //store all new packges
                if (!packageStore.Contains(rootDependency))
                {
                    packageStore.Add(rootDependency);
                }
                else
                {
                    continue;
                }

                foreach (var dependency in rootDependency.Dependencies)
                {
                    //currently we using restricted version during child dependency resolving 
                    //but possibly it should be configured in project
                    var relatedIdentity = new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion);

                    var relatedDepInfo = await dependencyInfoResource.ResolvePackage(relatedIdentity, targetFramework, cacheContext, _nugetLogger, cancellationToken);

                    if (relatedDepInfo != null)
                    {
                        downloadStack.Push(relatedDepInfo);
                        continue;
                    }

                    if (ignoreMissingPackages)
                    {
                        resolvedBehavior = DependencyBehavior.Ignore;
                        await _nugetLogger.LogAsync(LogLevel.Warning, $"Available sources doesn't contain package {relatedIdentity}. Package {relatedIdentity} is missing");
                    }
                    else
                    {
                        throw new MissingPackageException($"Cannot find package {relatedIdentity}");
                    }
                }
            }

            return resolvedBehavior;
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
            List<PackageIdentity> extractedPackages = new List<PackageIdentity>();

            try
            {
                var pathResolver = new PackagePathResolver(project.ContentPath);

                foreach (var packageResource in packageResources)
                {
                    var downloadedPart = packageResource.Value;
                    var packageIdentity = packageResource.Key;

                    var nupkgPath = pathResolver.GetInstalledPackageFilePath(packageIdentity);

                    bool alreadyInstalled = Directory.Exists(nupkgPath);

                    if (alreadyInstalled)
                    {
                        Log.Info($"Package {packageIdentity} already location in extraction directory");
                    }

                    Log.Info($"Extracting package {downloadedPart.GetResourceRoot()} to {project} project folder..");

                    var extractedPaths = await PackageExtractor.ExtractPackageAsync(
                        downloadedPart.PackageSource,
                        downloadedPart.PackageStream,
                        pathResolver,
                        extractionContext,
                        cancellationToken
                    );

                    Log.Info($"Successfully unpacked {extractedPaths.Count()} files");

                    if (!alreadyInstalled)
                    {
                        extractedPackages.Add(packageIdentity);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"An error occured during package extraction");

                var extractionEx = new ProjectInstallException(ex.Message, ex)
                {
                    CurrentBatch = extractedPackages
                };

                throw extractionEx;
            }
        }


        private PackageResolverContext GetResolverContext(PackageIdentity package, DependencyBehavior dependencyBehavior, IEnumerable<SourcePackageDependencyInfo> flatDependencies)
        {
            var idArray = new[] { package.Id };

            var requiredPackages = Enumerable.Empty<string>(); //new List<string>() { "ChameleonApi" };

            var packagesConfig = Enumerable.Empty<PackageReference>();

            var prefferedVersion = Enumerable.Empty<PackageIdentity>();

            var resolverContext = new PackageResolverContext(
                dependencyBehavior,
                idArray,
                requiredPackages,
                packagesConfig,
                prefferedVersion,
                flatDependencies,
                _sourceRepositoryProvider.GetRepositories().Select(x => x.PackageSource),
                _nugetLogger
            );

            return resolverContext;
        }

        private PackageExtractionContext GetExtractionContext()
        {
            //todo provide read certs?
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
            ICollection<SourcePackageDependencyInfo> markedForUninstall,
            IEnumerable<PackageIdentity> installedPackages,
            UninstallationContext uninstallationContext)
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
                        Log.Info($"{identity} package skipped, because one or more installed packages depends on it");
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
