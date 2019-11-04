namespace Orc.NuGetExplorer.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Enums;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class PageItemViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IRepositoryContextService _repositoryService;

        private readonly IModelProvider<ExplorerSettingsContainer> _settingsProvider;

        public PageItemViewModel(NuGetPackage package, IRepositoryContextService repositoryService, IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => settingsProvider);

            _repositoryService = repositoryService;
            _settingsProvider = settingsProvider;

            Package = package;
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

        [ViewModelToModel]
        public PackageStatus Status { get; set; }

        public bool IsDownloadCountShowed { get; private set; }

        public bool CanBeAddedInBatchOperation { get; set; }

        public bool IsChecked { get; set; }

        protected override Task InitializeAsync()
        {
            var packageOrigin = Package.FromPage;

            IsDownloadCountShowed = packageOrigin != MetadataOrigin.Installed;
            CanBeAddedInBatchOperation = packageOrigin == MetadataOrigin.Updates;

            return base.InitializeAsync();
        }
    }
}
