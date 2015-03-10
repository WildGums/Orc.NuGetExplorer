// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesUIService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;
    using NuGet;
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

            HttpClient.DefaultCredentialProvider = typeFactory.CreateInstance<NuGetSettingsCredentialProvider>();      
        }
        #endregion

        #region Methods
        public async Task ShowPackagesExplorer()
        {
            _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        }
        #endregion
    }
}