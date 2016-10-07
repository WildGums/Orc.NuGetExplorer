// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Catel;
    using Catel.MVVM;

    internal class PackageDetailsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageDetailsService _packageDetailsService;
        #endregion

        #region Constructors
        public PackageDetailsViewModel(IPackageDetails package, IPackageDetailsService packageDetailsService)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => packageDetailsService);

            _packageDetailsService = packageDetailsService;

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

            PackageSummary = _packageDetailsService.PackageToFlowDocument(Package);
        }
        #endregion
    }
}