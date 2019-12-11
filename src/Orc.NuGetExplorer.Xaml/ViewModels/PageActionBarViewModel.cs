namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGetExplorer.Management;
    using NuGetExplorer.Windows;
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

        public PageActionBarViewModel(IManagerPage managerPage, IProgressManager progressManager, INuGetPackageManager projectManager,
            IExtensibleProjectLocator projectLocator, IPackageCommandService packageCommandService)
        {
            Argument.IsNotNull(() => managerPage);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => projectLocator);
            Argument.IsNotNull(() => packageCommandService);

            _parentManagerPage = managerPage;
            _projectManager = projectManager;
            _projectLocator = projectLocator;
            _progressManager = progressManager;
            _packageCommandService = packageCommandService;

            BatchUpdate = new TaskCommand(BatchUpdateExecute, BatchUpdateCanExecute);
        }

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

        private async Task BatchUpdateExecute()
        {
            try
            {
                _progressManager.ShowBar(this);

                var batchedPackages = _parentManagerPage.PackageItems.Where(x => x.IsChecked).ToList();

                var projects = _projectLocator.GetAllExtensibleProjects()
                            .Where(x => _projectLocator.IsEnabled(x)).ToList();

                using (var cts = new CancellationTokenSource())
                {
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
                        await _packageCommandService.ExecuteUpdateAsync(updatePackageDetails, cts.Token);
                        //await _projectManager.UpdatePackageForProjectAsync(targetProjects.FirstOrDefault(), package.Identity.Id, targetVersion, cts.Token);
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
