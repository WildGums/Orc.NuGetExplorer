// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatchViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    internal class PackagesBatchViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageActionService _packageActionService;
        #endregion

        #region Constructors
        public PackagesBatchViewModel(PackagesBatch packagesBatch, IPackageActionService packageActionService)
        {
            Argument.IsNotNull(() => packagesBatch);
            Argument.IsNotNull(() => packageActionService);

            _packageActionService = packageActionService;

            PackagesBatch = packagesBatch;
            AccentColorHelper.CreateAccentColorResourceDictionary();

            ActionName = _packageActionService.GetActionName(packagesBatch.OperationType);

            PackageAction = new TaskCommand(OnPackageActionExecute, OnPackageActionCanExecute);
        }
        #endregion

        #region Properties
        [Model]
        [Expose("PackageList")]
        public PackagesBatch PackagesBatch { get; set; }

        public string ActionName { get; private set; }
        public PackageDetails SelectedPackage { get; set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            RefreshCanExecute();

            SetTitle();
        }

        private void SetTitle()
        {
            switch (PackagesBatch.OperationType)
            {
                case PackageOperationType.Install:
                    Title = "Installing packages";
                    break;

                case PackageOperationType.Uninstall:
                    Title = "Uninstalling packages";
                    break;

                case PackageOperationType.Update:
                    Title = "Updating packages";
                    break;
            }
        }
        #endregion

        #region Commands
        public TaskCommand PackageAction { get; set; }

        private async Task OnPackageActionExecute()
        {
            await _packageActionService.Execute(PackagesBatch.OperationType, SelectedPackage);

            RefreshCanExecute();
        }

        private bool OnPackageActionCanExecute()
        {
            return _packageActionService.CanExecute(PackagesBatch.OperationType, SelectedPackage);
        }

        private void RefreshCanExecute()
        {
            foreach (var package in PackagesBatch.PackageList)
            {
                package.IsActionExecuted = null;
                _packageActionService.CanExecute(PackagesBatch.OperationType, package);
            }
        }
        #endregion
    }
}