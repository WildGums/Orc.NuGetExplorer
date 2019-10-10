// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Example.ViewModels
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;

    public class SettingsViewModel : ViewModelBase
    {
        #region Fields
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        #endregion

        #region Constructors
        public SettingsViewModel(INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            _nuGetConfigurationService = nuGetConfigurationService;

            Title = "Package source settings";
        }
        #endregion

        #region Properties
        public IEnumerable<IPackageSource> PackageSources { get; set; }
        #endregion

        #region Methods
        protected override Task InitializeAsync()
        {
            PackageSources = _nuGetConfigurationService.LoadPackageSources();

            return base.InitializeAsync();
        }

        protected override async Task<bool> SaveAsync()
        {
            _nuGetConfigurationService.SavePackageSources(PackageSources);

            return await base.SaveAsync();
        }
        #endregion
    }
}