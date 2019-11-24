﻿namespace Orc.NuGetExplorer.Scenario
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
    using NuGet.Packaging.Core;
    using NuGet.ProjectManagement;
    using Orc.NuGetExplorer.Management;

    public class V3RestorePackageConfigAndReinstall : IUpgradeScenario
    {
        private const string Name = "Upgrade packages to compatible versions";

        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IExtensibleProject _defaultProject;
        private readonly INuGetPackageManager _nuGetPackageManager;
        private readonly IRepositoryContextService _repositoryContextService;
        private readonly ILogger _logger;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public V3RestorePackageConfigAndReinstall(IDefaultExtensibleProjectProvider projectProvider, INuGetPackageManager nuGetPackageManager, IRepositoryContextService repositoryContextService,
            ILogger logger)
        {
            Argument.IsNotNull(() => projectProvider);
            Argument.IsNotNull(() => nuGetPackageManager);
            Argument.IsNotNull(() => repositoryContextService);

            _defaultProject = projectProvider.GetDefaultProject();
            _nuGetPackageManager = nuGetPackageManager;
            _repositoryContextService = repositoryContextService;
            _logger = logger;
            _cancellationTokenSource = new CancellationTokenSource();
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

            bool anyUpgraded = false;

            using (var context = _repositoryContextService.AcquireContext())
            {
                foreach (var folder in subFolders)
                {
                    var packageFolderName = Path.GetFileName(folder);
                    var package = PackageIdentityParser.Parse(packageFolderName);

                    try
                    {
                        var isV2packageInstalled = package != null && folderProject.PackageExists(package, NuGet.Packaging.PackageSaveMode.Defaultv2);

                        if (!isV2packageInstalled)
                        {
                            Log.Warning($"Package {package} does not recognized in project folder as v2 NuGet installed package");
                            continue;
                        }

                        //reinstall
                        if (await _nuGetPackageManager.IsPackageInstalledAsync(_defaultProject, package, _cancellationTokenSource.Token))
                        {
                            Log.Info($"Skip package {package}, package is valid");
                            continue;
                        }

                        var isInstalled = await _nuGetPackageManager.InstallPackageForProjectAsync(_defaultProject, package, _cancellationTokenSource.Token);
                        
                        if(!isInstalled)
                        {
                            failedIdentities.Add(package);
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

                return anyUpgraded;
            }
        }



        public override string ToString()
        {
            //scenario name
            return Name;
        }
    }
}
