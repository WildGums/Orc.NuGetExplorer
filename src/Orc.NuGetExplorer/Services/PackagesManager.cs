// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using NuGet;

    internal class PackagesManager : IPackagesManager
    {
        #region Fields
        private readonly PackageManager _packageManager;
        #endregion

        #region Constructors
        public PackagesManager(INuGetConfigurationService nuGetConfigurationService,
            IPackageRepositoryService packageRepositoryService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            var repository = packageRepositoryService.GetAggregateRepository();
            var folder = nuGetConfigurationService.GetDestinationFolder();

            _packageManager = new PackageManager(repository, folder);
        }
        #endregion

        #region Methods
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

        public async Task Update(IPackage package)
        {
            Argument.IsNotNull(() => package);

            _packageManager.UpdatePackage(package.Id, false, true);
        }
        #endregion
    }
}