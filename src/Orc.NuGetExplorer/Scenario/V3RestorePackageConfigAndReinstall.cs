namespace Orc.NuGetExplorer.Scenario
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Packaging.Core;
    using NuGet.ProjectManagement;
    using Orc.FileSystem;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Packaging;
    using Settings = NuGetExplorer.Settings;

    public class V3RestorePackageConfigAndReinstall : IUpgradeScenario
    {
        private const string Name = "Upgrade packages to compatible versions";
        private const string FallbackUriKey = Settings.NuGet.FallbackUrl;

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IExtensibleProject _defaultProject;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IRepositoryContextService _repositoryContextService;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;
        private readonly IDirectoryService _directoryService;

        public V3RestorePackageConfigAndReinstall(IDefaultExtensibleProjectProvider projectProvider, INuGetPackageManager nuGetPackageManager, IRepositoryContextService repositoryContextService,
            ILogger logger, IConfigurationService configurationService, IPackageOperationNotificationService packageOperationNotificationService, IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => projectProvider);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => repositoryContextService);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => packageOperationNotificationService);
            Argument.IsNotNull(() => directoryService);

            _defaultProject = projectProvider.GetDefaultProject();
            _nuGetPackageManager = nuGetPackageManager;
            _repositoryContextService = repositoryContextService;
            _logger = logger;
            _configurationService = configurationService;
            _packageOperationNotificationService = packageOperationNotificationService;
            _directoryService = directoryService;
        }

        public async Task<bool> RunAsync()
        {
            var folderProject = new FolderNuGetProject(_defaultProject.ContentPath);

            if (!_directoryService.Exists(_defaultProject.ContentPath))
            {
                Log.Info($"Plugins folder does not exist");
                return false;
            }

            var subFolders = folderProject.GetPackageDirectories();

            var failedIdentities = new List<PackageIdentity>();
            var parsedPackages = new List<IPackageDetails>();

            bool anyUpgraded = false;

            using (var context = AcquireSourceContextForActions())
            {
                if (context == SourceContext.EmptyContext)
                {
                    Log.Warning($"Source context is empty");

                    return false;
                }

                foreach (var folder in subFolders)
                {
                    var packageFolderName = Path.GetFileName(folder);
                    var package = PackageIdentityParser.Parse(packageFolderName);

                    var packageDetails = PackageDetailsFactory.Create(package);

                    parsedPackages.Add(packageDetails);
                }

                //no valid package folders
                if (!parsedPackages.Any())
                {
                    return false;
                }

                _packageOperationNotificationService.NotifyAutomaticOperationBatchStarting(PackageOperationType.Install, parsedPackages.ToArray());

                foreach (var packageDetails in parsedPackages)
                {
                    var package = packageDetails.GetIdentity();
                    if (package is null)
                    {
                        continue;
                    }

                    var installationPath = _defaultProject.GetInstallPath(package);

                    var isV2packageInstalled = folderProject.PackageExists(package, NuGet.Packaging.PackageSaveMode.Defaultv2);
                    if (!isV2packageInstalled)
                    {
                        Log.Warning($"Package '{package}' is recognized in project folder as v2 NuGet installed package");
                        continue;
                    }

                    if (await _nuGetPackageManager.IsPackageInstalledAsync(_defaultProject, package, default))
                    {
                        Log.Info($"Skipping package '{package}', package is valid");
                        continue;
                    }

                    _packageOperationNotificationService.NotifyAutomaticOperationStarting(installationPath, PackageOperationType.Install, packageDetails);

                    //reinstall
                    try
                    {
                        var isInstalled = await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package, default, false);

                        if (!isInstalled)
                        {
                            failedIdentities.Add(package);
                        }

                        anyUpgraded = isInstalled || anyUpgraded;
                    }

                    catch (Exception ex)
                    {
                        failedIdentities.Add(package);
                        Log.Error(ex);
                    }

                    _packageOperationNotificationService.NotifyAutomaticOperationFinished(installationPath, PackageOperationType.Install, packageDetails);
                }

                await _logger.LogAsync(LogLevel.Information, $"Update completed. Package count {subFolders.Count()}");

                if (failedIdentities.Any())
                {
                    await _logger.LogAsync(LogLevel.Information, $"Failed to install some packages:");
                    failedIdentities.ForEach(async failed => await _logger.LogAsync(LogLevel.Information, failed.ToString()));
                }

                _packageOperationNotificationService.NotifyAutomaticOperationBatchFinished(PackageOperationType.Install, parsedPackages.ToArray());

                return anyUpgraded;
            }
        }

        public override string ToString()
        {
            //scenario name
            return Name;
        }

        private SourceContext AcquireSourceContextForActions()
        {
            var context = _repositoryContextService.AcquireContext(ignoreLocal: true);

            if (context == SourceContext.EmptyContext)
            {
                Log.Warning($"Source context is empty, trying to get fallback plugins Uri");

                context = CreateSourceFromFallbackPluginsUri();
            }

            return context;
        }

        private SourceContext CreateSourceFromFallbackPluginsUri()
        {
            var defaultPluginUri = _configurationService.GetRoamingValue<string>(FallbackUriKey);

            if (string.IsNullOrEmpty(defaultPluginUri))
            {
                return SourceContext.EmptyContext;
            }

            var context = _repositoryContextService.AcquireContext(new PackageSource(defaultPluginUri));

            return context;
        }
    }
}
