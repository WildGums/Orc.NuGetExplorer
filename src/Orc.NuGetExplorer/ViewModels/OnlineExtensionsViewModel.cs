// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel.MVVM;
    using NuGet;

    public class OnlineExtensionsViewModel : ViewModelBase
    {
        public OnlineExtensionsViewModel()
        {
            
        }

        public string SearchFilter { get; set; }

        public IServerPackageMetadata SelectedPackage { get; set; }

        private void OnSearchFilterChanged()
        {
            
        }
    }
}