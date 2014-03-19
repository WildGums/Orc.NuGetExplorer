// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Services;

    public class ExplorerViewModel : ViewModelBase
    {
        private readonly INuGetService _nuGetService;

        public ExplorerViewModel(INuGetService nuGetService)
        {
            Argument.IsNotNull(() => nuGetService);

            _nuGetService = nuGetService;
        }

        public string SelectedGroup { get; set; }
    }
}