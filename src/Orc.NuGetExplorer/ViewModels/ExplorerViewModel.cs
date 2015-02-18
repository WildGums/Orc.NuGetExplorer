// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using Catel;
    using Catel.MVVM;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private readonly INavigationTreeService _navigationTreeService;
        private readonly IPackageSourceService _packageSourceService;
        #endregion

        #region Constructors
        public ExplorerViewModel(IPackageSourceService packageSourceService, INavigationTreeService navigationTreeService)
        {
            Argument.IsNotNull(() => packageSourceService);
            Argument.IsNotNull(() => navigationTreeService);

            _packageSourceService = packageSourceService;
            _navigationTreeService = navigationTreeService;

            NavigationItems = new List<NavigationItem>(_navigationTreeService.GetNavigationItems());

            AvailablePackageSources = new List<string>();
            foreach (var packageSource in packageSourceService.PackageSources)
            {
                AvailablePackageSources.Add(packageSource.Name);
            }
        }
        #endregion

        #region Properties
        public IList<NavigationItem> NavigationItems { get; private set; }
        public string SelectedGroup { get; set; }
        public string SelectedPackageSource { get; set; }
        public List<string> AvailablePackageSources { get; private set; }
        #endregion
    }
}