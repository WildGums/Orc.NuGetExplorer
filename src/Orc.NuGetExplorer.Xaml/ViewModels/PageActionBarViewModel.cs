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

    internal class PageActionBarViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IManagerPage _parentManagerPage;

        private readonly INuGetPackageManager _projectManager;

        private readonly IExtensibleProjectLocator _projectLocator;

        private readonly IProgressManager _progressManager;

        public PageActionBarViewModel(IManagerPage managerPage, IProgressManager progressManager, INuGetPackageManager projectManager,
            IExtensibleProjectLocator projectLocator)
        {
            Argument.IsNotNull(() => managerPage);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => projectLocator);

            _parentManagerPage = managerPage;
            _projectManager = projectManager;
            _projectLocator = projectLocator;
            _progressManager = progressManager;

            BatchUpdate = new TaskCommand(BatchUpdateExecute, BatchUpdateCanExecute);
        }

        protected override Task InitializeAsync()
        {
            _parentManagerPage.PackageItems.CollectionChanged += OnParentPagePackageItemsCollectionChanged;
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

                        if (targetVersion == null)
                        {
                            throw new InvalidOperationException("Target version for update cannot be null");
                        }

                        await _projectManager.UpdatePackageForMultipleProject(targetProjects, package.Identity.Id, targetVersion, cts.Token);
                    }
                }

                _progressManager.HideBar(this);

                _parentManagerPage.StartLoadingTimerOrInvalidateData();
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error when updating package");
            }
        }

        private bool BatchUpdateCanExecute()
        {
            return _parentManagerPage.PackageItems.Any(); /*(x => x.IsChecked);*/
        }

        private void OnParentPagePackageItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
