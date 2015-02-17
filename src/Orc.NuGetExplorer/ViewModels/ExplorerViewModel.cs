// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using Catel;
    using Catel.MVVM;

    public class ExplorerViewModel : ViewModelBase
    {
        private readonly IPackageSourceService _packageSourceService;

        public ExplorerViewModel(IPackageSourceService packageSourceService)
        {
            Argument.IsNotNull(() => packageSourceService);

            _packageSourceService = packageSourceService;

            AvailablePackageSources = new List<string>();
            foreach (var packageSource in packageSourceService.PackageSources)
            {
                AvailablePackageSources.Add(packageSource.Name);
            }
        }

        public string SelectedGroup { get; set; }

        public string SelectedPackageSource { get; set; }

        public List<string> AvailablePackageSources { get; private set; }
    }
}