namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
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

        private readonly IProgressManager _progressManager;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IMessageService _messageService;

        public PageActionBarViewModel(IManagerPage managerPage, IProgressManager progressManager, IPackageCommandService packageCommandService, 
            IPackageOperationContextService packageOperationContextService, IMessageService messageService, ICommandManager commandManager)
        {
            Argument.IsNotNull(() => managerPage);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => packageOperationContextService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => commandManager);

            _parentManagerPage = managerPage;
            _progressManager = progressManager;
            _packageCommandService = packageCommandService;
            _packageOperationContextService = packageOperationContextService;
            _messageService = messageService;

            BatchInstall = new TaskCommand(BatchInstallExecuteAsync, BatchInstallCanExecute);
            CheckAll = new TaskCommand(CheckAllExecuteAsync);

            CanBatchInstall = _parentManagerPage.CanBatchInstallOperations;
            CanBatchUpdate = _parentManagerPage.CanBatchUpdateOperations;

            var batchUpdateCommand = (ICompositeCommand)commandManager.GetCommand(Commands.Packages.BatchUpdate);
            InvalidateCanBatchUpdateExecute = () => batchUpdateCommand.RaiseCanExecuteChanged();

            Parent = _parentManagerPage;
        }

        public bool IsCheckAll { get; set; }

        public bool CanBatchUpdate { get; set; }

        public bool CanBatchInstall { get; set; }

        public IManagerPage Parent { get; private set; }

        public Action InvalidateCanBatchUpdateExecute { get; set; }

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

        public TaskCommand BatchInstall { get; set; }

        private async Task BatchInstallExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                var batchedPackages = _parentManagerPage.PackageItems.Where(x => x.IsChecked).ToList();

                if (batchedPackages.Any(x => x.ValidationContext.HasErrors))
                {
                    await _messageService.ShowErrorAsync("One or more package(s) cannot be installed due to validation errors", "Can't install packages");
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

            BatchInstall.RaiseCanExecuteChanged();
            InvalidateCanBatchUpdateExecute();
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
