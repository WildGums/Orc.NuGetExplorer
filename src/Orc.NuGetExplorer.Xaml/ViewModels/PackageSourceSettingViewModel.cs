namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Collections;
    using Catel.Configuration;
    using Catel.Data;
    using Catel.Logging;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;

    internal class PackageSourceSettingViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly NuGetConfigurationService _configurationService;

        private readonly INuGetFeedVerificationService _feedVerificationService;

        private readonly IModelProvider<NuGetFeed> _modelProvider;

        public PackageSourceSettingViewModel(List<NuGetFeed> configuredFeeds, IConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService,
            IModelProvider<NuGetFeed> modelProvider) : this(configurationService, feedVerificationService, modelProvider)
        {
            Argument.IsNotNull(() => configuredFeeds);

            SettingsFeeds = configuredFeeds ?? new List<NuGetFeed>();
            Feeds.AddRange(SettingsFeeds);
        }

        public PackageSourceSettingViewModel(IConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService,
            IModelProvider<NuGetFeed> modelProvider)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => modelProvider);
            Argument.IsNotNull(() => feedVerificationService);

            _configurationService = configurationService as NuGetConfigurationService;
            _feedVerificationService = feedVerificationService;
            _modelProvider = modelProvider;

            RemovedFeeds = new List<NuGetFeed>();

            CommandInitialize();
            Title = "Settings";

            DeferValidationUntilFirstSaveCall = true;

            DefaultFeed = Constants.DefaultNugetOrgUri;
            DefaultSourceName = Constants.DefaultNugetOrgName;

            SettingsFeeds = new List<NuGetFeed>();
            Feeds = new ObservableCollection<NuGetFeed>();
        }

        public ObservableCollection<NuGetFeed> Feeds { get; set; }

        //[Model]
        public NuGetFeed SelectedFeed { get; set; }

        public List<NuGetFeed> RemovedFeeds { get; set; }

        /// <summary>
        /// All feeds currently loaded in settings model
        /// </summary>
        public List<NuGetFeed> SettingsFeeds { get; set; }

        #region ViewToViewModel

        public string DefaultFeed { get; set; }
        public string DefaultSourceName { get; set; }
        public IEnumerable<IPackageSource> PackageSources { get; set; }

        #endregion

        private bool ListenViewToViewModelPropertyChanges { get; set; } = true;

        #region Commands

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
            Feeds.Add(new NuGetFeed(DefaultSourceName, DefaultFeed));
        }

        #endregion

        protected void CommandInitialize()
        {
            RemoveFeed = new Command(OnRemoveFeedExecute);
            MoveUpFeed = new Command(OnMoveUpFeedExecute);
            MoveDownFeed = new Command(OnMoveDownFeedExecute);
            AddFeed = new Command(OnAddFeedExecute);
        }

        protected override async Task InitializeAsync()
        {
            _modelProvider.PropertyChanged += OnModelProviderPropertyChanged;
            Feeds.CollectionChanged += OnFeedsCollectioChanged;

            Feeds.ForEach(async x => await VerifyFeedAsync(x));
        }

        protected override Task<bool> SaveAsync()
        {
            SaveFeeds();

            ListenViewToViewModelPropertyChanges = false;

            PackageSources = Feeds.ToList();

            ListenViewToViewModelPropertyChanges = true;

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

        private void SaveFeeds()
        {
            SettingsFeeds.Clear();
            SettingsFeeds.AddRange(Feeds);

            _configurationService.SavePackageSources(Feeds);
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

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (!ListenViewToViewModelPropertyChanges)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(PackageSources)) && PackageSources != null)
            {
                var passedFeeds = PackageSources.OfType<NuGetFeed>().ToList();
                SettingsFeeds.AddRange(passedFeeds);
                Feeds.AddRange(passedFeeds);
            }

            base.OnPropertyChanged(e);
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
