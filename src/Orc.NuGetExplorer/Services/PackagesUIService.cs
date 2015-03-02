// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUIService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Services;
    using ViewModels;

    public class PackagesUIService : IPackagesUIService
    {
        private readonly IUIVisualizerService _uiVisualizerService;

        public PackagesUIService(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;
        }

        public async Task ShowPackagesExplorer()
        {
            await _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        } 
    }
}