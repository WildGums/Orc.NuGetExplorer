// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatchViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;

    internal class PackagesBatchViewModel : ViewModelBase
    {
        #region Constructors
        public PackagesBatchViewModel(PackagesBatch packagesBatch)
        {
            Argument.IsNotNull(() => packagesBatch);

            PackagesBatch = packagesBatch;
            AccentColorHelper.CreateAccentColorResourceDictionary();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("PackageList")]
        public PackagesBatch PackagesBatch { get; set; }
        #endregion
    }
}