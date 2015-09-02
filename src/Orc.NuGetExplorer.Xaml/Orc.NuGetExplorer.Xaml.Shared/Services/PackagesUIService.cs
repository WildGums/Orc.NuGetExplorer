// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUIService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;
    using ViewModels;

    internal class PackagesUIService : IPackagesUIService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
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
        public void ShowPackagesExplorer()
        {
            _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        }
        #endregion
    }
}