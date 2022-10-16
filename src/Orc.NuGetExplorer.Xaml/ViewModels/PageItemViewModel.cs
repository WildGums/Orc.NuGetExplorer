namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Catel;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGet.Versioning;
    using Orc.NuGetExplorer.Enums;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class PageItemViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly string InstalledVersionText = "Installed version";
        private static readonly string LastVersionText = "Latest version";
        private static readonly string UpdateVersionText = "Update version";
        private readonly ExplorerSettingsContainer _nugetSettings;

        public PageItemViewModel(NuGetPackage package, IModelProvider<ExplorerSettingsContainer> settingsProvider, ICommandManager commandManager)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => commandManager);

            Package = package;
            _nugetSettings = settingsProvider.Model;

            var batchUpdateCommand = (ICompositeCommand?)commandManager.GetCommand(Commands.Packages.BatchUpdate);
            if (batchUpdateCommand is null)
            {
                throw Log.ErrorAndCreateException<InvalidOperationException>($"Failed to get required command '{Commands.Packages.BatchUpdate}'");
            }
            InvalidateCanBatchUpdateExecute = () => batchUpdateCommand.RaiseCanExecuteChanged();

            //command
            CheckItem = new Command<MouseButtonEventArgs>(CheckItemExecute);
        }

        [Model(SupportIEditableObject = false)]
        [Expose("Title")]
        [Expose("Description")]
        [Expose("Summary")]
        [Expose("DownloadCount")]
        [Expose("Authors")]
        [Expose("IconUrl")]
        [Expose("Identity")]
        [Expose("FromPage")]
        public NuGetPackage Package { get; set; }

        [ViewModelToModel]
        public PackageStatus Status { get; set; }

        [ViewModelToModel]
        public bool IsChecked { get; set; }

        public bool IsDownloadCountShowed { get; private set; }

        public bool CanBeAddedInBatchOperation { get; set; }

        public NuGetVersion PrimaryVersion { get; set; }

        public NuGetVersion SecondaryVersion { get; set; }

        public string PrimaryVersionDescription { get; set; }

        public string SecondaryVersionDescription { get; set; }

        public Action InvalidateCanBatchUpdateExecute { get; }

        public Command<MouseButtonEventArgs> CheckItem { get; set; }

        private void CheckItemExecute(MouseButtonEventArgs parameter)
        {
            if (parameter.ClickCount < 2)
            {
                return;
            }

            IsChecked = !IsChecked;
        }

        protected override Task InitializeAsync()
        {
            // Handle only on Browse page
            if (ParentViewModel is ExplorerPageViewModel explorerParent)
            {
                if (explorerParent.Page.Parameters.Tab == ExplorerTab.Browse)
                {
                    Package.StatusChanged += OnPackageStatusChanged;
                    _nugetSettings.PropertyChanged += OnNuGetSettingsChanged;
                }
            }

            var packageOrigin = Package.FromPage;

            IsDownloadCountShowed = packageOrigin == MetadataOrigin.Browse;
            CanBeAddedInBatchOperation = packageOrigin != MetadataOrigin.Installed;

            GetPrimaryVersionInfo(Package);
            GetSecondaryVersionInfo(packageOrigin, Package);

            return base.InitializeAsync();
        }

        private void OnNuGetSettingsChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (!e.HasPropertyChanged(nameof(ExplorerSettingsContainer.IsHideInstalled)))
            {
                return;
            }

            if (Package.Status == PackageStatus.LastVersionInstalled || Package.Status == PackageStatus.UpdateAvailable)
            {
                Package.IsDelisted = _nugetSettings.IsHideInstalled;
            }
        }

        protected override Task CloseAsync()
        {
            // Handle only on Browse page
            if (ParentViewModel is ExplorerPageViewModel explorerParent)
            {
                if (explorerParent.Page.Parameters.Tab == ExplorerTab.Browse)
                {
                    Package.StatusChanged -= OnPackageStatusChanged;
                    _nugetSettings.PropertyChanged -= OnNuGetSettingsChanged;
                }
            }

            return base.CloseAsync();
        }

        private void OnPackageStatusChanged(object? sender, PackageModelStatusEventArgs e)
        {
            if (e.NewStatus == PackageStatus.LastVersionInstalled || e.NewStatus == PackageStatus.UpdateAvailable)
            {
                Package.IsDelisted = _nugetSettings.IsHideInstalled;
            }
        }

        protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.HasPropertyChanged(nameof(Package.LastVersion)))
            {
                GetSecondaryVersionInfo(Package.FromPage, Package);
            }

            if (e.HasPropertyChanged(nameof(Package.InstalledVersion)))
            {
                GetPrimaryVersionInfo(Package);
            }

            if (e.HasPropertyChanged(nameof(Package.IsChecked)))
            {
                InvalidateCanBatchUpdateExecute();
            }

            base.OnModelPropertyChanged(sender, e);
        }

        private void GetSecondaryVersionInfo(MetadataOrigin fromPage, NuGetPackage package)
        {
            SecondaryVersion = package.LastVersion;

            if (MetadataOrigin.Updates == fromPage)
            {
                SecondaryVersionDescription = $"{UpdateVersionText}: {SecondaryVersion}";
                return;
            }

            SecondaryVersionDescription = $"{LastVersionText}: {SecondaryVersion}";
        }

        private void GetPrimaryVersionInfo(NuGetPackage package)
        {
            PrimaryVersion = package.InstalledVersion;
            PrimaryVersionDescription = $"{InstalledVersionText}: {PrimaryVersion}";
        }
    }
}
