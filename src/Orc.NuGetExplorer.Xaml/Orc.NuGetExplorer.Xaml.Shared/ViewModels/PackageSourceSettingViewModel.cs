// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceSettingViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.MVVM;

    internal class PackageSourceSettingViewModel : ViewModelBase
    {
        #region Fields
        private readonly INuGetConfigurationService _nuGetConfigurationService;
        #endregion

        #region Constructors
        public PackageSourceSettingViewModel(INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            _nuGetConfigurationService = nuGetConfigurationService;

            Add = new TaskCommand(OnAddExecute);
            Remove = new TaskCommand(OnRemoveExecute, OnRemoveCanExecute);
        }
        #endregion

        #region Properties
        public IList<EditablePackageSource> PackageSources { get; private set; }
        public EditablePackageSource SelectedPackageSource { get; set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            var loadPackageSources = _nuGetConfigurationService.LoadPackageSources();

            PackageSources = new FastObservableCollection<EditablePackageSource>(loadPackageSources.Select(x =>
                new EditablePackageSource
                {
                    IsEnabled = x.IsEnabled,
                    Name = x.Name,
                    Source = x.Source
                }));

            await base.Initialize();
        }
        #endregion

        #region Commands
        public TaskCommand Add { get; private set; }

        private async Task OnAddExecute()
        {
            var packageSource = new EditablePackageSource
            {
                IsEnabled = true,
                Name = "New package source",
                Source = "Feed url"
            };

            PackageSources.Add(packageSource);
            SelectedPackageSource = packageSource;
        }

        public TaskCommand Remove { get; private set; }

        private async Task OnRemoveExecute()
        {
            if (SelectedPackageSource == null)
            {
                return;
            }

            var index = PackageSources.IndexOf(SelectedPackageSource);
            if (index < 0)
            {
                return;
            }

            PackageSources.RemoveAt(index);

            if (index < PackageSources.Count)
            {
                SelectedPackageSource = PackageSources[index];
            }
            else
            {
                SelectedPackageSource = PackageSources.LastOrDefault();
            }
        }

        private bool OnRemoveCanExecute()
        {
            return SelectedPackageSource != null;
        }
        #endregion
    }
}