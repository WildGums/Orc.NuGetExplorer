namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGetExplorer.Management;
    using NuGetExplorer.Windows;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Packaging;

    internal class PageActionBarViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IManagerPage _parentManagerPage;

        private readonly INuGetPackageManager _projectManager;
        private readonly IExtensibleProjectLocator _projectLocator;
        private readonly IProgressManager _progressManager;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageOperationContextService _packageOperationContextService;

        public PageActionBarViewModel(IManagerPage managerPage, IProgressManager progressManager, INuGetPackageManager projectManager,
            IExtensibleProjectLocator projectLocator, IPackageCommandService packageCommandService, IPackageOperationContextService packageOperationContextService)
        {
            Argument.IsNotNull(() => managerPage);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => projectLocator);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packageOperationContextService);

            _parentManagerPage = managerPage;
            _projectManager = projectManager;
            _projectLocator = projectLocator;
            _progressManager = progressManager;
            _packageCommandService = packageCommandService;
            _packageOperationContextService = packageOperationContextService;

            BatchUpdate = new TaskCommand(BatchUpdateExecuteAsync, BatchUpdateCanExecute);
            CheckAll = new TaskCommand(CheckAllExecuteAsync);
        }

        public bool IsCheckAll { get; set; }

        protected override Task InitializeAsync()
        {
            _parentManagerPage.PackageItems.CollectionChanged += OnParentPagePackageItemsCollectionChanged;
            NuGetPackage.AnyNuGetPackageCheckedChanged += OnAnyNuGetPackageCheckedChanged;
            return base.InitializeAsync();
        }

        protected override Task OnClosingAsync()
        {
            _parentManagerPage.PackageItems.CollectionChanged -= OnParentPagePackageItemsCollectionChanged;
            return base.OnClosingAsync();
        }

        public TaskCommand BatchUpdate { get; set; }

        private async Task BatchUpdateExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                var batchedPackages = _parentManagerPage.PackageItems.Where(x => x.IsChecked).ToList();

                var projects = _projectLocator.GetAllExtensibleProjects()
                            .Where(x => _projectLocator.IsEnabled(x)).ToList();

                using (var cts = new CancellationTokenSource())
                {
                    var updatePackageList = new List<IPackageDetails>();

                    foreach (var package in batchedPackages)
                    {
                        var targetProjects = new List<IExtensibleProject>();

                        foreach (var project in projects)
                        {
                            if (!await _projectManager.IsPackageInstalledAsync(project, package.Identity, cts.Token))
                            {
                                targetProjects.Add(project);
                            }
                        }

                        var targetVersion = (await package.LoadVersionsAsync() ?? package.Versions)?.FirstOrDefault();

                        if (targetVersion is null)
                        {
                            Log.Warning("Cannot perform upgrade because of 'Target version' is null");
                            return;
                        }


                        var updatePackageDetails = PackageDetailsFactory.Create(PackageOperationType.Update, package.GetMetadata(), targetVersion, null);
                        updatePackageList.Add(updatePackageDetails);
                    }

                    using (var operationContext = _packageOperationContextService.UseOperationContext(PackageOperationType.Update, updatePackageList.ToArray()))
                    {
                        foreach (var updatePackageDetails in updatePackageList)
                        {
                            await _packageCommandService.ExecuteUpdateAsync(updatePackageDetails, cts.Token, operationContext);
                        }
                    }
                }

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when updating package");
            }
            finally
            {
                _progressManager.HideBar(this);

                _parentManagerPage.StartLoadingTimerOrInvalidateData();
            }
        }

        private bool BatchUpdateCanExecute()
        {
            if (_parentManagerPage is null)
            {
                return false;
            }

            return _parentManagerPage.PackageItems.Any(x => x.IsChecked);
        }

        public TaskCommand CheckAll { get; set; }

        private async Task CheckAllExecuteAsync()
        {
            var packages = _parentManagerPage.PackageItems;
            packages.ForEach(package => package.IsChecked = IsCheckAll);
        }

        private void OnParentPagePackageItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            InvalidateCommands();
        }

        private void OnAnyNuGetPackageCheckedChanged(object sender, EventArgs e)
        {
            InvalidateCommands();
        }

        private void InvalidateCommands()
        {
            var commandManager = this.GetViewModelCommandManager();
            commandManager.InvalidateCommands();
        }
    }
}
