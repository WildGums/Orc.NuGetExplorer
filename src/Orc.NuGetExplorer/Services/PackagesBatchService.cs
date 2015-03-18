// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatchService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Catel;
    using Catel.Services;
    using NuGet;
    using ViewModels;

    internal class PackagesBatchService : IPackagesBatchService
    {
        private readonly IUIVisualizerService _uiVisualizerService;

        public PackagesBatchService(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;
        }

        public void ShowPackagesBatch(ObservableCollection<IPackageDetails> packageDetails, PackageOperationType operationType)
        {
            var packagesBatch = new PackagesBatch { OperationType = PackageOperationType.Update };
            packagesBatch.PackageList.AddRange(packageDetails.Cast<PackageDetails>());
            _uiVisualizerService.ShowDialog<PackagesBatchViewModel>(packagesBatch);
        }
    }
}