// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using NuGet;
    using ViewModels;

    internal class PackagesManager : IPackagesManager
    {
        #region Fields
        private readonly IUIVisualizerService _uiVisualizerService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        private PackageManager _packageManager;
        #endregion

        #region Constructors
        public PackagesManager(IUIVisualizerService uiVisualizerService, IPackageQueryService packageQueryService, INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => nuGetConfigurationService);

            _uiVisualizerService = uiVisualizerService;
            _packageQueryService = packageQueryService;
            _nuGetConfigurationService = nuGetConfigurationService;


        }
        #endregion

        #region Methods
        public async Task Show()
        {
            await _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        }

        public async Task Install(IPackage package, IPackageRepository packageRepository, FrameworkName targetFramework = null)
        {
            Argument.IsNotNull(() => package);
            
            var folder = _nuGetConfigurationService.GetDestinationFolder();

            _packageManager = new PackageManager(packageRepository, folder);
            _packageManager.InstallPackage(package, false, true);
           
        }
        #endregion
    }
}