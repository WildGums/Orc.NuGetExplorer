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
     //   private readonly IHttpClientFactory _httpClientFactory;
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        #endregion

        #region Constructors
        public PackagesUIService(IUIVisualizerService uiVisualizerService, ITypeFactory typeFactory/*, IHttpClientFactory httpClientFactory*/,
            IPackageRepositoryFactory packageRepositoryFactory)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => typeFactory);
          //  Argument.IsNotNull(() => httpClientFactory);
            Argument.IsNotNull(() => packageRepositoryFactory);

            _uiVisualizerService = uiVisualizerService;
          //  _httpClientFactory = httpClientFactory;
            _packageRepositoryFactory = packageRepositoryFactory;

            HttpClient.DefaultCredentialProvider = typeFactory.CreateInstance<NuGetSettingsCredentialProvider>();
            var repositoryFactory = packageRepositoryFactory as PackageRepositoryFactory;
            if (repositoryFactory != null)
            {
                //repositoryFactory.HttpClientFactory = _httpClientFactory.GetFactory();
            }
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