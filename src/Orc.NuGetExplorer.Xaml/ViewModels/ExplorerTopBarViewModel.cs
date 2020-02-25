namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Models;

    internal class ExplorerTopBarViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;

        private readonly IUIVisualizerService _uIVisualizerService;

        private readonly INuGetCacheManager _nuGetCacheManager;

        private readonly IPleaseWaitService _pleaseWaitService;

        private readonly IMessageService _messageService;

        private readonly INuGetConfigurationService _configurationService;

        public ExplorerTopBarViewModel(ExplorerSettingsContainer settings, ITypeFactory typeFactory, IUIVisualizerService uIVisualizerService, INuGetConfigurationService configurationService,
            INuGetCacheManager nuGetCacheManager, IPleaseWaitService pleaseWaitService, IMessageService messageService)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => uIVisualizerService);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => settings);
            Argument.IsNotNull(() => nuGetCacheManager);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => messageService);

            _typeFactory = typeFactory;
            _uIVisualizerService = uIVisualizerService;
            _configurationService = configurationService;
            _nuGetCacheManager = nuGetCacheManager;
            _pleaseWaitService = pleaseWaitService;
            _messageService = messageService;

            Settings = settings;

            Title = "Manage NuGet Packages";
            CommandInitialize();
        }

        [Model(SupportIEditableObject = false)]
        public ExplorerSettingsContainer Settings { get; set; }

        [ViewModelToModel]
        public bool IsPreReleaseIncluded { get; set; }

        [ViewModelToModel]
        public string SearchString { get; set; }

        [ViewModelToModel]
        public INuGetSource ObservedFeed { get; set; }

        [ViewModelToModel]
        public INuGetSource DefaultFeed { get; set; }

        public bool SelectFirstPageOnLoad { get; set; } = true;

        public ObservableCollection<INuGetSource> ActiveFeeds { get; set; }

        protected override Task InitializeAsync()
        {
            ActiveFeeds = new ObservableCollection<INuGetSource>(GetActiveFeedsFromSettings());

            //"all" feed is default
            DefaultFeed = ActiveFeeds.FirstOrDefault(x => string.Equals(x.Name, Constants.CombinedSourceName));

            ObservedFeed = SetObservedFeed(ActiveFeeds, DefaultFeed);

            return base.InitializeAsync();
        }

        protected void CommandInitialize()
        {
            ShowPackageSourceSettings = new TaskCommand(OnShowPackageSourceSettingsExecuteAsync);
            ShowExtensibles = new TaskCommand(OnShowExtensiblesAsync);
            RunNuGetCachesClearing = new TaskCommand(OnRunNuGetCachesClearingAsync);
        }

        public TaskCommand ShowPackageSourceSettings { get; set; }

        private async Task OnShowPackageSourceSettingsExecuteAsync()
        {
            var nugetSettingsVm = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<NuGetSettingsViewModel>();

            if (nugetSettingsVm != null)
            {
                var result = await _uIVisualizerService.ShowDialogAsync(nugetSettingsVm);

                if (result ?? false)
                {
                    //update available feeds
                    ActiveFeeds = new ObservableCollection<INuGetSource>(GetActiveFeedsFromSettings());
                }
            }
        }

        public TaskCommand ShowExtensibles { get; set; }

        private async Task OnShowExtensiblesAsync()
        {
            var extensiblesVM = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExtensiblesViewModel>();

            if (extensiblesVM != null)
            {
                var result = await _uIVisualizerService.ShowDialogAsync(extensiblesVM);
            }
        }

        public TaskCommand RunNuGetCachesClearing { get; set; }

        private async Task OnRunNuGetCachesClearingAsync()
        {
            try
            {
                var shouldRunClear = await _messageService.ShowAsync("Clean all NuGet caches, including global packages folder?", "NuGet Package Management", MessageButton.YesNo);

                if (shouldRunClear == MessageResult.No)
                {
                    return;
                }

                _pleaseWaitService.Push();

                var noErrors = _nuGetCacheManager.ClearAll();

                _pleaseWaitService.Pop();

                if (noErrors)
                {
                    await _messageService.ShowInformationAsync(Constants.Messages.CacheClearEndedSuccessful, Constants.PackageManagement);
                }
                else
                {
                    await _messageService.ShowWarningAsync(Constants.Messages.CachedClearEndedWithError, Constants.PackageManagement);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, Constants.Messages.CacheClearFailed);

                await _messageService.ShowErrorAsync(Constants.Messages.CacheClearFailed, Constants.PackageManagement);
            }
        }

        private IEnumerable<INuGetSource> GetActiveFeedsFromSettings()
        {
            var activefeeds = Settings.NuGetFeeds.Where(x => x.IsEnabled).ToList<INuGetSource>();

            var allInOneSource = new CombinedNuGetSource(activefeeds);

            activefeeds.Insert(0, allInOneSource);

            return activefeeds;
        }

        private INuGetSource SetObservedFeed(IEnumerable<INuGetSource> feeds, INuGetSource defaultFeed)
        {
            var lastSelectedSourceName = (_configurationService as IConfigurationService)?.GetLastRepository("Browse") ?? string.Empty;

            return feeds.FirstOrDefault(x => string.Equals(x.Name, lastSelectedSourceName)) ?? defaultFeed;
        }
    }
}
