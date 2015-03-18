// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageBatchService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Services;
    using NuGet;
    using ViewModels;

    internal class PackageBatchService : IPackageBatchService
    {
        #region Fields
        private readonly IDispatcherService _dispatcherService;
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors
        public PackageBatchService(IUIVisualizerService uiVisualizerService, IDispatcherService dispatcherService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => dispatcherService);

            _uiVisualizerService = uiVisualizerService;
            _dispatcherService = dispatcherService;
        }
        #endregion

        #region Methods
        public void ShowPackagesBatch(IEnumerable<IPackageDetails> packageDetails, PackageOperationType operationType)
        {
            var packagesBatch = new PackagesBatch {OperationType = PackageOperationType.Update};
            packagesBatch.PackageList.AddRange(packageDetails.Cast<PackageDetails>());

            _dispatcherService.Invoke(() => _uiVisualizerService.ShowDialogAsync<PackageBatchViewModel>(packagesBatch));
        }
        #endregion
    }
}