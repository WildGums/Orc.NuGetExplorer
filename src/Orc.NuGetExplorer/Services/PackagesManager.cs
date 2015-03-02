// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using NuGet;
    using ViewModels;

    internal class PackagesManager : IPackagesManager
    {
        #region Fields
        private readonly PackageManager _packageManager;
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public PackagesManager(IUIVisualizerService uiVisualizerService, INuGetConfigurationService nuGetConfigurationService,
            IPackageRepositoryService packageRepositoryService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => nuGetConfigurationService);

            _uiVisualizerService = uiVisualizerService;

            var repository = packageRepositoryService.GetAggregateRepository();
            var folder = nuGetConfigurationService.GetDestinationFolder();

            _packageManager = new PackageManager(repository, folder);
        }
        #endregion

        #region Methods
        public async Task Show()
        {
            await _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        }

        public async Task Install(IPackage package)
        {
            Argument.IsNotNull(() => package);

            _packageManager.InstallPackage(package, false, true);
        }

        public async Task Uninstall(IPackage package)
        {
            Argument.IsNotNull(() => package);

            _packageManager.UninstallPackage(package);
        }
        #endregion
    }
}