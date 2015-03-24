// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageBatchViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    internal class PackageBatchViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageOperationContextService _packageOperationContextService;
        #endregion

        #region Constructors
        public PackageBatchViewModel(PackagesBatch packagesBatch, IPackageCommandService packageCommandService, IPackageOperationContextService packageOperationContextService)
        {
            Argument.IsNotNull(() => packagesBatch);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packageOperationContextService);

            _packageCommandService = packageCommandService;
            _packageOperationContextService = packageOperationContextService;

            PackagesBatch = packagesBatch;
            AccentColorHelper.CreateAccentColorResourceDictionary();

            ActionName = _packageCommandService.GetActionName(packagesBatch.OperationType);
            PluralActionName = _packageCommandService.GetPluralActionName(packagesBatch.OperationType);

            PackageAction = new TaskCommand(OnPackageActionExecute, OnPackageActionCanExecute);
            ApplyAll = new TaskCommand(OnApplyAllExecute, OnApplyAllCanExecute);
        }
        #endregion

        #region Properties
        [Model]
        [Expose("PackageList")]
        public PackagesBatch PackagesBatch { get; set; }

        public string ActionName { get; private set; }
        public string PluralActionName { get; private set; }
        public IPackageDetails SelectedPackage { get; set; }
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
        public TaskCommand ApplyAll { get; private set; }

        private async Task OnApplyAllExecute()
        {
            var packages = PackagesBatch.PackageList.Where(p => _packageCommandService.CanExecute(PackagesBatch.OperationType, p)).Cast<IPackageDetails>().ToArray();
            using (_packageOperationContextService.UseOperationContext(PackagesBatch.OperationType, packages))
            {
                foreach (var package in packages)
                {
                    await _packageCommandService.Execute(PackagesBatch.OperationType, package);
                    RefreshCanExecute();
                }
            }            
        }

        private bool OnApplyAllCanExecute()
        {
            return PackagesBatch.PackageList.All(p => _packageCommandService.CanExecute(PackagesBatch.OperationType, p));
        }

        public TaskCommand PackageAction { get; set; }

        private async Task OnPackageActionExecute()
        {
            await _packageCommandService.Execute(PackagesBatch.OperationType, SelectedPackage);

            RefreshCanExecute();
        }

        private bool OnPackageActionCanExecute()
        {
            return _packageCommandService.CanExecute(PackagesBatch.OperationType, SelectedPackage);
        }

        private void RefreshCanExecute()
        {
            foreach (var package in PackagesBatch.PackageList)
            {
                package.IsInstalled = null;
                _packageCommandService.CanExecute(PackagesBatch.OperationType, package);
            }
        }
        #endregion
    }
}