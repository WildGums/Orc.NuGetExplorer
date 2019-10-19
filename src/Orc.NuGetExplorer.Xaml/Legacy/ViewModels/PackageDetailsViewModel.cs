// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.ComponentModel;
    using System.IO.Packaging;
    using System.Threading.Tasks;
    using System.Windows.Documents;

    using Catel;
    using Catel.Data;
    using Catel.MVVM;

    internal class PackageDetailsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageDetailsService _packageDetailsService;
        private readonly IPackageQueryService _packageQueryService;
        private readonly IRepositoryNavigatorService _repositoryNavigatorService;
        #endregion

        #region Constructors
        public PackageDetailsViewModel(IPackageDetails package, IPackageDetailsService packageDetailsService, IPackageQueryService packageQueryService, IRepositoryNavigatorService repositoryNavigatorService)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => packageDetailsService);
            Argument.IsNotNull(() => packageQueryService);
            Argument.IsNotNull(() => repositoryNavigatorService);

            _packageDetailsService = packageDetailsService;
            _packageQueryService = packageQueryService;
            _repositoryNavigatorService = repositoryNavigatorService;

            Package = package;
        }
        #endregion

        #region Properties
        [Model(SupportIEditableObject = false)]
        public IPackageDetails Package { get; private set; }

        public FlowDocument PackageSummary { get; private set; }
        #endregion

        #region Methods
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (Package is ModelBase modelBase)
            {
                modelBase.PropertyChanged += (sender, args) =>
                    {
                        var selectedVersionPropertyName = nameof(Package.SelectedVersion);
                        if (args.HasPropertyChanged(selectedVersionPropertyName))
                        {
                            BuildPackageSummary();
                        }
                    };
            }

            BuildPackageSummary();
        }

        private void BuildPackageSummary()
        {
            //// Fix: Required since available versions aren't available until dropdown button is displayed.
            if (!string.IsNullOrWhiteSpace(Package.SelectedVersion) && Package.Version.ToString() != Package.SelectedVersion)
            {
                var packageSummary = _packageQueryService.GetPackage(_repositoryNavigatorService.Navigator.SelectedRepository, Package.Id, Package.SelectedVersion);
                PackageSummary = _packageDetailsService.PackageToFlowDocument(packageSummary);
            }
            else
            {
                PackageSummary = _packageDetailsService.PackageToFlowDocument(Package);
            }
        }
        #endregion
    }
}
