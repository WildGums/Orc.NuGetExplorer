namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Data;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using NuGetExplorer.Enums;
    using NuGetExplorer.Management;
    using NuGetExplorer.Models;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;
    using NuGetExplorer.Windows;

    internal class PackageDetailsViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly int Timeout = 500;

        private readonly IRepositoryContextService _repositoryService;

        private readonly IModelProvider<ExplorerSettingsContainer> _settingsProvider;

        private readonly IProgressManager _progressManager;

        private readonly INuGetPackageManager _projectManager;

        private IPackageMetadataProvider _packageMetadataProvider;

        public PackageDetailsViewModel(IRepositoryContextService repositoryService, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            IProgressManager progressManager, INuGetPackageManager projectManager)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => projectManager);

            _repositoryService = repositoryService;
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;
            _projectManager = projectManager;

            LoadInfoAboutVersions = new Command(LoadInfoAboutVersionsExecute, () => Package != null);
            InstallPackage = new TaskCommand(OnInstallPackageExecute, OnInstallPackageCanExecute);
            UninstallPackage = new TaskCommand(OnUninstallPackageExecute, OnUninstallPackageCanExecute);
        }

        [Model(SupportIEditableObject = false)]
        [Expose("Title")]
        [Expose("Description")]
        [Expose("Summary")]
        [Expose("DownloadCount")]
        [Expose("Authors")]
        [Expose("IconUrl")]
        [Expose("Identity")]
        public NuGetPackage Package { get; set; }

        public ObservableCollection<NuGetVersion> VersionsCollection { get; set; }

        public object DependencyInfo { get; set; }

        public DeferToken DefferedLoadingToken { get; set; }

        [ViewModelToModel]
        public PackageStatus Status { get; set; }

        public NuGetActionTarget NuGetActionTarget { get; } = new NuGetActionTarget();

        public IPackageSearchMetadata VersionData { get; set; }

        public NuGetVersion SelectedVersion { get; set; }

        public PackageIdentity SelectedPackage => Package is null ? null : new PackageIdentity(Package.Identity.Id, SelectedVersion);

        public PackageIdentity InstalledPackage => Package is null ? null : new PackageIdentity(Package.Identity.Id, InstalledVersion);

        [ViewModelToModel]
        public NuGetVersion InstalledVersion { get; set; }

        public int SelectedVersionIndex { get; set; }

        private void OnPackageChanged()
        {
            if (Package is null)
            {
                return;
            }

#pragma warning disable 4014
            ApplyPackageAsync();
#pragma warning restore 4014
        }

        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            if (Package is null)
            {
                return;
            }

            await ApplyPackageAsync();

            NuGetActionTarget.PropertyChanged += OnNuGetActionTargetPropertyPropertyChanged;
        }

        private async Task ApplyPackageAsync()
        {
            try
            {
                //select identity version
                var selectedVersion = Package?.Identity?.Version;

                VersionsCollection = new ObservableCollection<NuGetVersion>() { selectedVersion };

                SelectedVersion = selectedVersion;

                _packageMetadataProvider = InitMetadataProvider();

                await LoadSinglePackageMetadataAsync(Package.Identity);
            }
            catch (Exception e)
            {
                Log.Error(e, "Error ocurred during view model inititalization, probably package metadata is incorrect");
            }
        }

        protected override Task CloseAsync()
        {
            NuGetActionTarget.PropertyChanged -= OnNuGetActionTargetPropertyPropertyChanged;

            return base.CloseAsync();
        }

        protected async Task LoadSinglePackageMetadataAsync(PackageIdentity identity)
        {
            try
            {
                using (var cts = new CancellationTokenSource())
                {
                    VersionData = await _packageMetadataProvider?.GetPackageMetadataAsync(
                        identity, _settingsProvider.Model.IsPreReleaseIncluded, cts.Token);

                    DependencyInfo = VersionData?.DependencySets;

                    Package.AddDependencyInfo(VersionData.Identity.Version, VersionData?.DependencySets);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Metadata retrieve error");
            }
        }


        public Command LoadInfoAboutVersions { get; set; }

        private void LoadInfoAboutVersionsExecute()
        {
            try
            {
                PopulateVersionCollection();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public TaskCommand InstallPackage { get; set; }

        private async Task OnInstallPackageExecute()
        {
            try
            {
                _progressManager.ShowBar(this);

                using (var cts = new CancellationTokenSource())
                {
                    await _projectManager.InstallPackageForMultipleProject(NuGetActionTarget.TargetProjects, SelectedPackage, cts.Token);
                }

                await Task.Delay(200);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error when installing package {Package.Identity}, installation was failed");
            }
            finally
            {
                _progressManager.HideBar(this);
            }
        }

        private bool OnInstallPackageCanExecute()
        {
            var anyProject = NuGetActionTarget?.IsValid ?? false;

            return anyProject && !IsInstalled();
        }

        public TaskCommand UninstallPackage { get; set; }

        private async Task OnUninstallPackageExecute()
        {
            try
            {
                _progressManager.ShowBar(this);

                using (var cts = new CancellationTokenSource())
                {
                    //InstalledPackage means you cannot directly choose version which should be uninstalled, may be this should be revised
                    await _projectManager.UninstallPackageForMultipleProject(NuGetActionTarget.TargetProjects, InstalledPackage, cts.Token);
                }

                await Task.Delay(200);
            }
            catch (Exception e)
            {
                Log.Error(e, $"Error when uninstalling package {Package.Identity}, uninstall was failed");
            }
            finally
            {
                _progressManager.HideBar(this);
            }
        }

        private bool OnUninstallPackageCanExecute()
        {
            var anyProject = NuGetActionTarget?.IsValid ?? false;

            return anyProject && IsInstalled();
        }

        private IPackageMetadataProvider InitMetadataProvider()
        {
            var currentSourceContext = SourceContext.CurrentContext;

            var repositories = currentSourceContext.Repositories ?? currentSourceContext?.PackageSources.Select(src => _repositoryService.GetRepository(src));

            return new PackageMetadataProvider(repositories, null);
        }

        private void PopulateVersionCollection()
        {
            try
            {
                if (Package.LoadVersionsAsync().Wait(Timeout))
                {
                    VersionsCollection = new ObservableCollection<NuGetVersion>(Package.Versions);
                }
                else
                {
                    throw new TimeoutException();
                }
            }
            catch (TimeoutException ex)
            {
                Log.Error(ex, $"Failed to get package versions for a given time ({Timeout} ms)");
            }
        }

        private void OnNuGetActionTargetPropertyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var commandManager = this.GetViewModelCommandManager();
            commandManager.InvalidateCommands();
        }

        protected async override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (string.Equals(e.PropertyName, nameof(SelectedVersion)))
            {
                if (e.OldValue == null && SelectedVersion == Package.Identity.Version)
                {
                    //skip loading on version list first load
                    return;
                }

                var identity = new PackageIdentity(Package.Identity.Id, SelectedVersion);

                try
                {
                    await LoadSinglePackageMetadataAsync(identity);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Unexpected error when loading package metadata");
                }
            }
        }

        private bool IsInstalled()
        {
            return Status == PackageStatus.UpdateAvailable || Status == PackageStatus.LastVersionInstalled;
        }
    }
}
