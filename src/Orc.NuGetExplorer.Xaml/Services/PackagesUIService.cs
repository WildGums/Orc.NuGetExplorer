// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUIService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Services;
    using ViewModels;

    internal class PackagesUIService : IPackagesUIService
    {
        #region Fields
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public PackagesUIService(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);

            _uiVisualizerService = uiVisualizerService;
        }
        #endregion

        #region Methods
        public async Task ShowPackagesExplorerAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<ExplorerViewModel>();
        }

        public async Task ShowPackagesSourceSettingsAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<SettingsViewModel>(true);
        }
        #endregion
    }
}
