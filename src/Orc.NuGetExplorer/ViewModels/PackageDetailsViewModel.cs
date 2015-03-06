// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using System.Windows.Documents;
    using Catel;
    using Catel.MVVM;

    public class PackageDetailsViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackageDetailsService _packageDetailsService;
        #endregion

        #region Constructors
        public PackageDetailsViewModel(PackageDetails package, IPackageDetailsService packageDetailsService)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => packageDetailsService);

            _packageDetailsService = packageDetailsService;

            Package = package;
        }
        #endregion

        #region Properties
        [Model(SupportIEditableObject = false)]
        public PackageDetails Package { get; private set; }

        public FlowDocument PackageSummary { get; private set; }
        #endregion

        #region Methods
        protected override async Task Initialize()
        {
            await base.Initialize();

            PackageSummary = await _packageDetailsService.PackageToFlowDocument(Package.Package);
        }

        private static Bold CreateTitle(string text)
        {
            return text.ToInline().Bold();
        }
        #endregion
    }
}