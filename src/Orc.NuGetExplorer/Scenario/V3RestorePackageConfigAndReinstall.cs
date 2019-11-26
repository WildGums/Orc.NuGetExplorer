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
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Packaging;

    public class V3RestorePackageConfigAndReinstall : IUpgradeScenario
    {
        private const string Name = "Upgrade packages to compatible versions";
        private const string FallbackUriKey = "Plugins.FeedUrl";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IExtensibleProject _defaultProject;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IRepositoryContextService _repositoryContextService;
        private readonly ILogger _logger;
        private readonly IConfigurationService _configurationService;
        private readonly IPackageOperationNotificationService _packageOperationNotificationService;

        public V3RestorePackageConfigAndReinstall(IDefaultExtensibleProjectProvider projectProvider, INuGetPackageManager nuGetPackageManager, IRepositoryContextService repositoryContextService,
            ILogger logger, IConfigurationService configurationService, IPackageOperationNotificationService packageOperationNotificationService)
        {
            Argument.IsNotNull(() => projectProvider);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => repositoryContextService);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => packageOperationNotificationService);

            _defaultProject = projectProvider.GetDefaultProject();
            _nuGetPackageManager = nuGetPackageManager;
            _repositoryContextService = repositoryContextService;
            _logger = logger;
            _configurationService = configurationService;
            _packageOperationNotificationService = packageOperationNotificationService;
        }

        public async Task<bool> Run()
        {
            var folderProject = new FolderNuGetProject(_defaultProject.ContentPath);

            if (!Directory.Exists(_defaultProject.ContentPath))
            {
                Log.Info($"plugins folder does not exist");
                return false;
            }

            var subFolders = folderProject.GetPackageDirectories();

            List<PackageIdentity> failedIdentities = new List<PackageIdentity>();
            List<IPackageDetails> parsedPackages = new List<IPackageDetails>();

            bool anyUpgraded = false;

            using (var context = AcquireSourceContextForActions())
            {
                if (context == SourceContext.EmptyContext)
                {
                    Log.Warning($"Source context is empty");

                    return false;
                }

                foreach(var folder in subFolders)
                {
                    var packageFolderName = Path.GetFileName(folder);
                    var package = PackageIdentityParser.Parse(packageFolderName);

                    var packageDetails = PackageDetailsFactory.Create(package);

                    parsedPackages.Add(packageDetails);
                }

                _packageOperationNotificationService.NotifyOperationBatchStarting(PackageOperationType.Install, parsedPackages.ToArray());

                foreach (var packageDetails in parsedPackages)
                {
                    var package = packageDetails.GetIdentity();

                    try
                    {
                        var isV2packageInstalled = package != null && folderProject.PackageExists(package, NuGet.Packaging.PackageSaveMode.Defaultv2);

                        if (!isV2packageInstalled)
                        {
                            Log.Warning($"Package {package} does not recognized in project folder as v2 NuGet installed package");
                            continue;
                        }

                        //reinstall
                        if (await _nuGetPackageManager.IsPackageInstalledAsync(_defaultProject, package, default))
                        {
                            Log.Info($"Skip package {package}, package is valid");
                            continue;
                        }

                        _packageOperationNotificationService.NotifyOperationStarting(_defaultProject.GetInstallPath(package), PackageOperationType.Install, packageDetails);

                        var isInstalled = await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package, default);

                        _packageOperationNotificationService.NotifyOperationFinished(_defaultProject.GetInstallPath(package), PackageOperationType.Install, packageDetails);

                        if (!isInstalled)
                        {
                            failedIdentities.Add(package);
                            continue;
                        }

                        anyUpgraded = isInstalled || anyUpgraded;
                    }

                    catch (Exception e)
                    {
                        failedIdentities.Add(package);
                        Log.Error(e);
                    }
                }

                await _logger.LogAsync(LogLevel.Information, $"Update completed. Package count {subFolders.Count()}");

                if (failedIdentities.Any())
                {
                    await _logger.LogAsync(LogLevel.Information, $"Failed to install some packages:");
                    failedIdentities.ForEach(async failed => await _logger.LogAsync(LogLevel.Information, failed.ToString()));
                }

                _packageOperationNotificationService.NotifyOperationBatchFinished(PackageOperationType.Install, parsedPackages.ToArray());

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

            if(string.IsNullOrEmpty(defaultPluginUri))
            {
                return SourceContext.EmptyContext;
            }

            var context = _repositoryContextService.AcquireContext(new PackageSource(defaultPluginUri));

            return context;
        }
    }
}
