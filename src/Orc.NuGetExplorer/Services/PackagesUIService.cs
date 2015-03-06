// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUIService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using ViewModels;

    internal class PackagesUIService : IPackagesUIService
    {
        #region Fields
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public PackagesUIService(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;
        }
        #endregion

        #region Methods
        public async Task ShowPackagesExplorer()
        {
            try
            {
                await _uiVisualizerService.ShowDialog<ExplorerViewModel>();

            }
            catch (Exception exception)
            {
                
                //throw;
            }
            
        }
        #endregion
    }
}