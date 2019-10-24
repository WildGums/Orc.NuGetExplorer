// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageBatchService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel;
    using Catel.Collections;
    using Catel.Services;
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
            var packagesBatch = new PackagesBatch
            {
                OperationType = PackageOperationType.Update
            };

            ((ICollection<IPackageDetails>)packagesBatch.PackageList).AddRange(packageDetails);

            _dispatcherService.Invoke(() => _uiVisualizerService.ShowDialogAsync<PackageBatchViewModel>(packagesBatch), true);
        }
        #endregion
    }
}