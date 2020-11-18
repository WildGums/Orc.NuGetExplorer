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
    using Catel.Services;
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
        private readonly IMessageService _messageService;

        public PageActionBarViewModel(IManagerPage managerPage, IProgressManager progressManager, INuGetPackageManager projectManager,
            IExtensibleProjectLocator projectLocator, IPackageCommandService packageCommandService, IPackageOperationContextService packageOperationContextService, IMessageService messageService)
        {
            Argument.IsNotNull(() => managerPage);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => projectLocator);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => messageService);

            _parentManagerPage = managerPage;
            _projectManager = projectManager;
            _projectLocator = projectLocator;
            _progressManager = progressManager;
            _packageCommandService = packageCommandService;
            _packageOperationContextService = packageOperationContextService;
            _messageService = messageService;
            BatchUpdate = new TaskCommand(BatchUpdateExecuteAsync, BatchUpdateCanExecute);
            BatchInstall = new TaskCommand(BatchInstallExecuteAsync, BatchInstallCanExecute);
            CheckAll = new TaskCommand(CheckAllExecuteAsync);

            CanBatchInstall = _parentManagerPage.CanBatchInstallOperations;
            CanBatchUpdate = _parentManagerPage.CanBatchUpdateOperations;
        }

        public bool IsCheckAll { get; set; }

        public bool CanBatchUpdate { get; set; }

        public bool CanBatchInstall { get; set; }

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

        #region Commands

        public TaskCommand BatchUpdate { get; set; }

        private async Task BatchUpdateExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                var batchedPackages = _parentManagerPage.PackageItems.Where(x => x.IsChecked).ToList();

                if (batchedPackages.Any(x => x.ValidationContext.HasErrors))
                {
                    await _messageService.ShowErrorAsync("Can't perform update. One or multiple package cannot be updated due to validation errors", "Can't update packages");
                    return;
                }

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
                            await _packageCommandService.ExecuteUpdateAsync(updatePackageDetails, operationContext, cts.Token);
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
            return AnyItemOnPageChecked();
        }

        public TaskCommand BatchInstall { get; set; }

        private async Task BatchInstallExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                var batchedPackages = _parentManagerPage.PackageItems.Where(x => x.IsChecked).ToList();

                if (batchedPackages.Any(x => x.ValidationContext.HasErrors))
                {
                    await _messageService.ShowErrorAsync("Can't perform install. One or multiple package cannot be installed due to validation errors", "Can't install packages");
                    return;
                }

                using (var cts = new CancellationTokenSource())
                {
                    var installPackageList = new List<IPackageDetails>();

                    foreach (var package in batchedPackages)
                    {
                        var targetVersion = (await package.LoadVersionsAsync() ?? package.Versions)?.OrderByDescending(x => x).FirstOrDefault();

                        var installPackageDetails = PackageDetailsFactory.Create(PackageOperationType.Install, package.GetMetadata(), targetVersion, null);
                        installPackageList.Add(installPackageDetails);
                    }

                    using (var operationContext = _packageOperationContextService.UseOperationContext(PackageOperationType.Install, installPackageList.ToArray()))
                    {
                        foreach (var packageDetails in installPackageList)
                        {
                            await _packageCommandService.ExecuteInstallAsync(packageDetails, operationContext, cts.Token);
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

        private bool BatchInstallCanExecute()
        {
            return AnyItemOnPageChecked();
        }

        public TaskCommand CheckAll { get; set; }

        private async Task CheckAllExecuteAsync()
        {
            var packages = _parentManagerPage.PackageItems;
            packages.ForEach(package => package.IsChecked = IsCheckAll);
        }

        #endregion

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

        private bool AnyItemOnPageChecked()
        {
            if (_parentManagerPage is null)
            {
                return false;
            }

            return _parentManagerPage.PackageItems.Any(x => x.IsChecked);
        }
    }
}
