// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetailsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    public class PackageDetailsViewModel : ViewModelBase
    {
        #region Constructors
        public PackageDetailsViewModel(PackageDetails package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
        }
        #endregion

        #region Properties
        public override string Title
        {
            get { return Package.Title; }
        }

        [Model(SupportIEditableObject = false)]
        [Expose("Summary")]
        [Expose("IconUrl")]
        public PackageDetails Package { get; private set; }
        #endregion
    }
}