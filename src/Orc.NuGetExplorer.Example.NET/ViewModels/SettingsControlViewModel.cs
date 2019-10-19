using Orc.NuGetExplorer.Models;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;

namespace Orc.NuGetExplorer.ViewModels
{
    using Catel;
    using Catel.Collections;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Logging;
    using Catel.MVVM;
    using NuGetExplorer.Models;
    using NuGetExplorer.Providers;
    using NuGetExplorer.Services;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class SettingsControlViewModel : ViewModelBase
    {
        private static readonly ILog _log = LogManager.GetCurrentClassLogger();

        private readonly NugetConfigurationService _configurationService;

        private readonly INuGetFeedVerificationService _feedVerificationService;

        private readonly IModelProvider<NuGetFeed> _modelProvider;

        public SettingsControlViewModel(List<NuGetFeed> configredFeeds, IConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService,
            IModelProvider<NuGetFeed> modelProvider)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => modelProvider);
            Argument.IsNotNull(() => feedVerificationService);
            Argument.IsNotNull(() => configredFeeds);

            ActiveFeeds = configredFeeds;
            Feeds = new ObservableCollection<NuGetFeed>(ActiveFeeds);
            RemovedFeeds = new List<NuGetFeed>();

            _configurationService = configurationService as NugetConfigurationService;
            _feedVerificationService = feedVerificationService;
            _modelProvider = modelProvider;
            CommandInitialize();
            Title = "Settings";

            DeferValidationUntilFirstSaveCall = true;
        }

        public ObservableCollection<NuGetFeed> Feeds { get; set; }

        //[Model]
        public NuGetFeed SelectedFeed { get; set; }

        /// <summary>
        /// Feeds which should be visible for NuGet Package Manager
        /// </summary>
        public List<NuGetFeed> ActiveFeeds { get; set; }

        public List<NuGetFeed> RemovedFeeds { get; set; }

        public Command RemoveFeed { get; set; }

        private void OnRemoveFeedExecute()
        {
            RemovedFeeds.Add(SelectedFeed);
            Feeds.Remove(SelectedFeed);
        }

        public Command MoveUpFeed { get; set; }

        private void OnMoveUpFeedExecute()
        {
            Feeds.MoveUp(SelectedFeed);
        }

        public Command MoveDownFeed { get; set; }

        private void OnMoveDownFeedExecute()
        {
            Feeds.MoveDown(SelectedFeed);
        }

        public Command AddFeed { get; set; }

        private void OnAddFeedExecute()
        {
            Feeds.Add(new NuGetFeed(Constants.DefaultNugetOrgName, Constants.DefaultNugetOrgUri));
        }

        protected void CommandInitialize()
        {
            RemoveFeed = new Command(OnRemoveFeedExecute);
            MoveUpFeed = new Command(OnMoveUpFeedExecute);
            MoveDownFeed = new Command(OnMoveDownFeedExecute);
            AddFeed = new Command(OnAddFeedExecute);
        }

        protected override async Task InitializeAsync()
        {
            //handle manual model save on child viewmodel
            _modelProvider.PropertyChanged += OnModelProviderPropertyChanged;
            Feeds.CollectionChanged += OnFeedsCollectioChanged;

            Feeds.ForEach(async x => await VerifyFeedAsync(x));
        }

        protected override Task<bool> SaveAsync()
        {
            //store all feed inside configuration
            for (int i = 0; i < Feeds.Count; i++)
            {
                _configurationService.SetRoamingValueWithDefaultIdGenerator(Feeds[i]);
            }

            //send usable feeds (including failed)
            ActiveFeeds.Clear();
            ActiveFeeds.AddRange(Feeds);

            //feeds removal
            _configurationService.RemoveValues(ConfigurationContainer.Roaming, RemovedFeeds);
            RemovedFeeds.Clear();

            return base.SaveAsync();
        }

        protected override Task CloseAsync()
        {
            _modelProvider.PropertyChanged -= OnModelProviderPropertyChanged;
            return base.CloseAsync();
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (SelectedFeed != null && IsNamesNotUniqueRule(out var names))
            {
                foreach (var name in names)
                {
                    validationResults.Add(BusinessRuleValidationResult.CreateError($"Two or more feeds have same name '{name}'"));
                }
            }
        }

        private bool IsNamesNotUniqueRule(out IEnumerable<string> invalidNames)
        {
            var names = new List<string>();

            var groups = Feeds.GroupBy(x => x.Name).Where(g => g.Count() > 1);

            groups.ForEach(g => names.Add(g.Key));

            invalidNames = names;

            return groups.Count() > 0;
        }

        private async Task VerifyFeedAsync(NuGetFeed feed)
        {
            if (feed == null || !feed.IsValid())
            {
                return;
            }

            feed.IsVerifiedNow = true;

            using (var cts = new CancellationTokenSource())
            {
                var result = await _feedVerificationService.VerifyFeedAsync(feed.Source, cts.Token, true);
                feed.VerificationResult = result;
            }

            feed.IsVerifiedNow = false;
        }


        private void OnModelProviderPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var index = Feeds.IndexOf(SelectedFeed);

            Feeds[index] = _modelProvider.Model;

            SelectedFeed = _modelProvider.Model;
        }


        private async void OnFeedsCollectioChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //verify all new feeds in collection
            //because of feed edit is simple re-insertion we should'nt handle property change inside model
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add || e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (NuGetFeed item in e.NewItems)
                    {
                        if (!item.IsLocal() && item.VerificationResult == FeedVerificationResult.Unknown)
                        {
                            await VerifyFeedAsync(item);
                        }
                    }
                }
            }
        }
    }
}
