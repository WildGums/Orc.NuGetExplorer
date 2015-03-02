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
            
/*            var version = Environment.Version;
            if (targetFramework == null)
            {
                
                var supportedFrameworks = package.GetSupportedFrameworks();
                targetFramework = supportedFrameworks.FirstOrDefault(x => x.Version.Major == version.Major && x.Version.Minor == version.Minor);
            }

            if (targetFramework == null)
            {
                throw new NotSupportedInPlatformException(string.Format("Framework version {0} does notsupport by package {1}.",
                    version, package.GetFullName()));
            }*/

          //  var dependencies = package.GetCompatiblePackageDependencies(targetFramework);

            // TODO: Need to implement getting dependency package before
            /*  
            var packages = _packageQueryService.GetPackages(packageRepository, dependencies);
            foreach (var dependecyPackage in packages)
            {
                await Install(dependecyPackage, packageRepository, targetFramework);
            }*/

            var folder = _nuGetConfigurationService.GetDestinationFolder();
          //  folder = Path.Combine(folder, package.Id);

         //   var packageFiles = package.GetFiles();

            // TODO: add downloading package
           // var downloadUrl = ((NuGet.DataServicePackage) (package)).DownloadUrl;

            var packageManager = new PackageManager(packageRepository, folder);
            packageManager.InstallPackage(package, false, true);
           
        }
        #endregion
    }
}