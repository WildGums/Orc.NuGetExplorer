using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Services;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Catel.Services;
    using NuGetExplorer.Cache;
    using NuGetExplorer.Models;
    using NuGetExplorer.Services;
    using Orc.Notifications;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;

    public class ExplorerTopBarViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ITypeFactory _typeFactory;

        private readonly IUIVisualizerService _uIVisualizerService;

        private readonly INuGetCacheManager _nuGetCacheManager;

        private readonly IPleaseWaitService _pleaseWaitService;

        private readonly IMessageService _messageService;

        private readonly NugetConfigurationService _configurationService;

        private readonly INotificationService _notificationService;

        public ExplorerTopBarViewModel(ExplorerSettingsContainer settings, ITypeFactory typeFactory, IUIVisualizerService uIVisualizerService, IConfigurationService configurationService,
            INuGetCacheManager nuGetCacheManager, IPleaseWaitService pleaseWaitService, IMessageService messageService, INotificationService notificationService)
        {
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => uIVisualizerService);
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => settings);
            Argument.IsNotNull(() => nuGetCacheManager);
            Argument.IsNotNull(() => pleaseWaitService);
            Argument.IsNotNull(() => messageService);
            Argument.IsNotNull(() => notificationService);

            _typeFactory = typeFactory;
            _uIVisualizerService = uIVisualizerService;
            _configurationService = configurationService as NugetConfigurationService;
            _nuGetCacheManager = nuGetCacheManager;
            _pleaseWaitService = pleaseWaitService;
            _messageService = messageService;
            _notificationService = notificationService;

            Settings = settings;

            Title = "Manage Packages";
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

        public bool SelectFirstPageOnLoad { get; set; } = true;

        public ObservableCollection<INuGetSource> ActiveFeeds { get; set; }

        protected override Task InitializeAsync()
        {
            ReadFeedsFromConfiguration(Settings);

            //Log.Info("No feeds stored in configuration");
            //AddDefaultFeeds(Settings);

            ActiveFeeds = new ObservableCollection<INuGetSource>(GetActiveFeedsFromSettings());

            ObservedFeed = ActiveFeeds.FirstOrDefault(x => x.IsSelected);

            return base.InitializeAsync();
        }

        protected void CommandInitialize()
        {
            ShowPackageSourceSettings = new TaskCommand(OnShowPackageSourceSettingsExecuteAsync);
            ShowExtensibles = new TaskCommand(OnShowExtensibles);
            RunNuGetCachesClearing = new TaskCommand(OnRunNuGetCachesClearing);
        }

        public TaskCommand ShowPackageSourceSettings { get; set; }

        private async Task OnShowPackageSourceSettingsExecuteAsync()
        {
            var nugetSettingsVm = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<SettingsViewModel>(Settings);

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

        private async Task OnShowExtensibles()
        {
            var extensiblesVM = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<ExtensiblesViewModel>();

            if (extensiblesVM != null)
            {
                var result = await _uIVisualizerService.ShowDialogAsync(extensiblesVM);
            }
        }

        public TaskCommand RunNuGetCachesClearing { get; set; }

        private async Task OnRunNuGetCachesClearing()
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
            catch (Exception e)
            {
                Log.Error(e, Constants.Messages.CacheClearFailed);

                await _messageService.ShowErrorAsync(Constants.Messages.CacheClearFailed, Constants.PackageManagement);
            }
        }

        private void ReadFeedsFromConfiguration(ExplorerSettingsContainer settings)
        {
            NuGetFeed temp = null; ;

            var keyCollection = _configurationService.GetAllKeys(ConfigurationContainer.Roaming);

            for (int i = 0; i < keyCollection.Count; i++)
            {
                temp = _configurationService.GetRoamingValue(keyCollection[i]);

                if (temp != null)
                {
                    settings.NuGetFeeds.Add(temp);
                }
                else
                {
                    Log.Error($"Configuration value under key {i} is broken and cannot be loaded");
                }
            }
        }

        private void AddDefaultFeeds(ExplorerSettingsContainer settings)
        {
            settings.NuGetFeeds.Add(
                new NuGetFeed(
                  Constants.DefaultNugetOrgName,
                  Constants.DefaultNugetOrgUri
              ));
        }

        private IEnumerable<INuGetSource> GetActiveFeedsFromSettings()
        {
            var activefeeds = Settings.NuGetFeeds.Where(x => x.IsActive).ToList<INuGetSource>();
            var allInOneSource = new CombinedNuGetSource(activefeeds);

            activefeeds.Insert(0, allInOneSource);

            return activefeeds;
        }
    }
}
