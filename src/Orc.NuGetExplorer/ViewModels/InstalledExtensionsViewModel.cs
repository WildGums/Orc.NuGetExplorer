// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledExtensionsViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel.MVVM;

    internal class InstalledExtensionsViewModel : ViewModelBase
    {
        #region Properties
        public PackageSourcesNavigationItem PackageSource { get; set; }
        #endregion
    }
}