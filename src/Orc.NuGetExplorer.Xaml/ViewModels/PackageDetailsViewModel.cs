﻿namespace Orc.NuGetExplorer.ViewModels
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
    using Orc.FileSystem;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Packaging;

    internal class PackageDetailsViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly int Timeout = 500;

        private static IPackageMetadataProvider PackageMetadataProvider;

        private readonly IRepositoryContextService _repositoryService;
        private readonly IModelProvider<ExplorerSettingsContainer> _settingsProvider;
        private readonly IProgressManager _progressManager;
        private readonly IApiPackageRegistry _apiPackageRegistry;
        private readonly IPackageCommandService _packageCommandService;
        private readonly IDirectoryService _directoryService;

        public PackageDetailsViewModel(IRepositoryContextService repositoryService, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            IProgressManager progressManager, IApiPackageRegistry apiPackageRegistry, IPackageCommandService packageCommandService,
            IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => progressManager);
            Argument.IsNotNull(() => apiPackageRegistry);
            Argument.IsNotNull(() => packageCommandService);
            Argument.IsNotNull(() => directoryService);

            _repositoryService = repositoryService;
            _settingsProvider = settingsProvider;
            _progressManager = progressManager;
            _apiPackageRegistry = apiPackageRegistry;
            _packageCommandService = packageCommandService;
            _directoryService = directoryService;

            LoadInfoAboutVersions = new Command(LoadInfoAboutVersionsExecute, () => Package is not null);
            InstallPackage = new TaskCommand(OnInstallPackageExecuteAsync, OnInstallPackageCanExecute);
            UninstallPackage = new TaskCommand(OnUninstallPackageExecuteAsync, OnUninstallPackageCanExecute);
        }

        private bool IsPackageApplied { get; set; }

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

        [ViewModelToModel]
        public IValidationContext ValidationContext { get; set; }

        public NuGetActionTarget NuGetActionTarget { get; } = new NuGetActionTarget();

        public IPackageSearchMetadata VersionData { get; set; }

        public NuGetVersion SelectedVersion { get; set; }

        public PackageIdentity SelectedPackage => Package is null ? null : new PackageIdentity(Package.Identity.Id, SelectedVersion);

        public PackageIdentity InstalledPackage => Package is null ? null : new PackageIdentity(Package.Identity.Id, InstalledVersion);

        [ViewModelToModel]
        public NuGetVersion InstalledVersion { get; set; }

        public string[] ApiValidationMessages { get; private set; }

        #region Commands

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

        private async Task OnInstallPackageExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                using (var cts = new CancellationTokenSource())
                {
                    if (IsInstalled())
                    {
                        var updatePackageDetails = PackageDetailsFactory.Create(PackageOperationType.Update, VersionData, SelectedVersion, null);
                        await _packageCommandService.ExecuteUpdateAsync(updatePackageDetails, cts.Token);
                    }
                    else
                    {
                        var installPackageDetails = PackageDetailsFactory.Create(PackageOperationType.Install, VersionData, SelectedPackage, null);
                        await _packageCommandService.ExecuteInstallAsync(installPackageDetails, cts.Token);
                    }
                }

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when installing package {Package.Identity}, installation was failed");
            }
            finally
            {
                _progressManager.HideBar(this);
            }
        }

        private bool OnInstallPackageCanExecute()
        {
            if (!IsPackageApplied)
            {
                return false;
            }

            if (!IsProjectValid())
            {
                return false;
            }

            return !(Package?.ValidationContext.HasErrors ?? false) && !IsVersionInstalled();
        }

        public TaskCommand UninstallPackage { get; set; }

        private async Task OnUninstallPackageExecuteAsync()
        {
            try
            {
                _progressManager.ShowBar(this);

                using (var cts = new CancellationTokenSource())
                {
                    // InstalledPackage means you cannot directly choose version which should be uninstalled, may be this should be revised
                    var uninstallPackageDetails = PackageDetailsFactory.Create(PackageOperationType.Uninstall, Package.GetMetadata(), InstalledPackage, null);
                    await _packageCommandService.ExecuteUninstallAsync(uninstallPackageDetails, cts.Token);
                }

                await Task.Delay(200);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error when uninstalling package {Package.Identity}, uninstall was failed");
            }
            finally
            {
                _progressManager.HideBar(this);
            }
        }

        private bool OnUninstallPackageCanExecute()
        {
            if (!IsPackageApplied)
            {
                return false;
            }

            if (!IsProjectValid())
            {
                return false;
            }

            return IsInstalled();
        }

        #endregion

        protected static async Task<IPackageSearchMetadata> LoadSinglePackageMetadataAsync(PackageIdentity identity, NuGetPackage packageModel, bool isPreReleaseIncluded)
        {
            try
            {
                var versionMetadata = await PackageMetadataProvider?.GetPackageMetadataAsync(
                    identity, isPreReleaseIncluded, CancellationToken.None);

                if (versionMetadata?.Identity?.Version is not null)
                {
                    packageModel.AddDependencyInfo(versionMetadata.Identity.Version, versionMetadata.DependencySets);
                }

                return versionMetadata;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Metadata retrieve error");
                return null;
            }
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

        protected override Task CloseAsync()
        {
            NuGetActionTarget.PropertyChanged -= OnNuGetActionTargetPropertyPropertyChanged;

            return base.CloseAsync();
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
                if ((e.OldValue is null && SelectedVersion == Package.Identity.Version) || e.NewValue is null)
                {
                    // Skip loading on version list first load
                    return;
                }

                if (!IsPackageApplied)
                {
                    // Skip until model is applied
                    return;
                }

                var identity = new PackageIdentity(Package.Identity.Id, SelectedVersion);

                VersionData = await LoadSinglePackageMetadataAsync(identity, Package, _settingsProvider.Model.IsPreReleaseIncluded);

                if (Package is not null)
                {
                    // Note: Workaround, this is a hack way to set specific version of package
                    var tempPackage = new NuGetPackage(VersionData, Package.FromPage);
                    tempPackage.AddDependencyInfo(VersionData.Identity.Version, VersionData.DependencySets);
                    ValidateCurrentPackage(tempPackage);
                }
            }
        }

        private async void OnPackageChanged()
        {
            Log.Debug("Package changed");

            IsPackageApplied = false;

            if (Package is null)
            {
                return;
            }

            await ApplyPackageAsync();
        }

        private void OnVersionDataChanged()
        {
            DependencyInfo = VersionData?.DependencySets;
        }

        private async Task ApplyPackageAsync()
        {
            try
            {
                //select identity version
                var selectedVersion = Package?.Identity?.Version;

                VersionsCollection = new ObservableCollection<NuGetVersion>() { selectedVersion };

                SelectedVersion = selectedVersion;

                PackageMetadataProvider = InitMetadataProvider();

                VersionData = await LoadSinglePackageMetadataAsync(Package.Identity, Package, _settingsProvider.Model.IsPreReleaseIncluded);

                if (Package is not null)
                {
                    ValidateCurrentPackage(Package);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error ocurred during view model inititalization, probably package metadata is incorrect");
            }
            finally
            {
                IsPackageApplied = true;
            }
        }

        private void ValidateCurrentPackage(NuGetPackage package)
        {
            Argument.IsNotNull(() => package);

            //validate loaded dependencies
            package.ResetValidationContext();
            _apiPackageRegistry.Validate(package);

            GetPackageValidationErrors(package);

            // Note: this is a workaround to pass validation context from specific version package to main model
            if (!ReferenceEquals(Package, package))
            {
                ValidationContext = package.ValidationContext;
            }
        }

        private IPackageMetadataProvider InitMetadataProvider()
        {
            var currentSourceContext = SourceContext.CurrentContext;

            var repositories = currentSourceContext.Repositories ?? currentSourceContext?.PackageSources.Select(src => _repositoryService.GetRepository(src));

            return new PackageMetadataProvider(_directoryService, repositories, null);
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

        private bool IsProjectValid()
        {
            return NuGetActionTarget?.IsValid ?? false;
        }

        private bool IsInstalled()
        {
            return InstalledVersion is not null;
        }

        private bool IsVersionInstalled()
        {
            return InstalledVersion == SelectedVersion;
        }

        private void GetPackageValidationErrors(NuGetPackage package)
        {
            Argument.IsNotNull(() => package);

            // title: NuGetExplorer_PackageDetailsService_PackageToFlowDocument_GetAlertRecords_Errors
            ApiValidationMessages = package.ValidationContext.GetAlertMessages(ValidationTags.Api);
        }
    }
}
