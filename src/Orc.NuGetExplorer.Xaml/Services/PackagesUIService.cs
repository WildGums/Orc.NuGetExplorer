﻿// --------------------------------------------------------------------------------------------------------------------
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
        private readonly ITypeFactory _typeFactory;
        #endregion

        #region Constructors
        public PackagesUIService(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);

            _uiVisualizerService = uiVisualizerService;
            _typeFactory = typeFactory;

            SettingsTitle = null;
        }
        #endregion

        #region Properties

        /// <summary>
        /// Overriden title for settings window
        /// </summary>
        public string SettingsTitle { get; set; }

        #endregion

        #region Methods
        public async Task ShowPackagesExplorerAsync()
        {
            await _uiVisualizerService.ShowDialogAsync<ExplorerViewModel>();
        }

        public async Task ShowPackagesExplorerAsync(ExplorerTab openTab, PackageSearchParameters searchParameters)
        {
            var explorerVM = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExplorerViewModel>();
            explorerVM.ChangeStartPage(openTab.Name);
            explorerVM.ChangeInitialSearchParameters(openTab.Name, searchParameters);
            await _uiVisualizerService.ShowDialogAsync(explorerVM);
        }


        public async Task ShowPackagesSourceSettingsAsync()
        {
            var settingsVM = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<SettingsViewModel>(true, SettingsTitle);
            await _uiVisualizerService.ShowDialogAsync(settingsVM);
        }
        #endregion
    }
}
