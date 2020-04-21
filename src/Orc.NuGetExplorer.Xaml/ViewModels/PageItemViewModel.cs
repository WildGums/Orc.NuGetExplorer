namespace Orc.NuGetExplorer.ViewModels
{
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

    internal class PageItemViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly string InstalledVersionText = "Installed version";
        private static readonly string LastVersionText = "Latest version";
        private static readonly string UpdateVersionText = "Update version";

        public PageItemViewModel(NuGetPackage package)
        {
            Argument.IsNotNull(() => package);

            Package = package;

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
            var packageOrigin = Package.FromPage;

            IsDownloadCountShowed = packageOrigin == MetadataOrigin.Browse;
            CanBeAddedInBatchOperation = packageOrigin == MetadataOrigin.Updates;

            GetPrimaryVersionInfo(packageOrigin, Package);
            GetSecondaryVersionInfo(packageOrigin, Package);

            return base.InitializeAsync();
        }

        protected override void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(Package.InstalledVersion)))
            {
                GetSecondaryVersionInfo(Package.FromPage, Package);
            }

            base.OnModelPropertyChanged(sender, e);
        }

        private void GetSecondaryVersionInfo(MetadataOrigin fromPage, NuGetPackage package)
        {
            if (MetadataOrigin.Browse == fromPage)
            {
                SecondaryVersion = package.InstalledVersion;
                SecondaryVersionDescription = $"{InstalledVersionText}: {SecondaryVersion}";
                return;
            }

            SecondaryVersion = package.LastVersion;

            if (MetadataOrigin.Updates == fromPage)
            {
                SecondaryVersionDescription = $"{UpdateVersionText}: {SecondaryVersion}";
                return;
            }

            SecondaryVersionDescription = $"{LastVersionText}: {SecondaryVersion}";
        }

        private void GetPrimaryVersionInfo(MetadataOrigin fromPage, NuGetPackage package)
        {
            if (MetadataOrigin.Browse == fromPage)
            {
                PrimaryVersion = package.Identity.Version;
                PrimaryVersionDescription = $"{LastVersionText}: {PrimaryVersion}";
                return;
            }

            PrimaryVersion = package.InstalledVersion;
            PrimaryVersionDescription = $"{InstalledVersionText}: {PrimaryVersion}";
        }
    }
}
