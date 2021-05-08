namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using Catel;
    using Catel.Collections;
    using Catel.Data;
    using Catel.Logging;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Models;

    internal class PackageSourceSettingViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly int ValidationDelay = 800;
        private static readonly int VerificationBatch = 5;

        private readonly INuGetConfigurationService _configurationService;
        private readonly INuGetFeedVerificationService _feedVerificationService;

        private readonly Queue<NuGetFeed> _validationQueue = new Queue<NuGetFeed>();

        private static readonly System.Timers.Timer ValidationTimer = new System.Timers.Timer(ValidationDelay);

        private readonly INuGetConfigurationResetService _nuGetConfigurationResetService;

        public PackageSourceSettingViewModel(INuGetConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => feedVerificationService);

            _configurationService = configurationService;
            _feedVerificationService = feedVerificationService;

            RemovedFeeds = new List<NuGetFeed>();

            DeferValidationUntilFirstSaveCall = true;

            DefaultFeed = Constants.DefaultNuGetOrgUri;
            DefaultSourceName = Constants.DefaultNuGetOrgName;

            SettingsFeeds = new List<NuGetFeed>();
            Feeds = new ObservableCollection<NuGetFeed>();

            Title = "Settings";

            InitializeCommands();
        }

        public PackageSourceSettingViewModel(INuGetConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService, INuGetConfigurationResetService nuGetConfigurationResetService)
            : this(configurationService, feedVerificationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationResetService);
            _nuGetConfigurationResetService = nuGetConfigurationResetService;
        }

        public ObservableCollection<NuGetFeed> Feeds { get; set; }

        public NuGetFeed SelectedFeed { get; set; }

        public List<NuGetFeed> RemovedFeeds { get; set; }

        /// <summary>
        /// All feeds currently loaded in settings model
        /// </summary>
        public List<NuGetFeed> SettingsFeeds { get; private set; }

        #region ViewToViewModel

        public bool CanReset { get; set; }
        public string DefaultFeed { get; set; }
        public string DefaultSourceName { get; set; }
        public IEnumerable<IPackageSource> PackageSources { get; set; }

        #endregion

        private bool SupressFeedVerificationOnCollectionChanged { get; set; } = true;

        private bool IsVerifying { get; set; }

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
            Feeds.Add(new NuGetFeed(DefaultSourceName, DefaultFeed, true));
        }

        public TaskCommand Reset { get; private set; }

        private async Task OnResetExecuteAsync()
        {
            if (_nuGetConfigurationResetService is null)
            {
                return;
            }
            await _nuGetConfigurationResetService.ResetAsync();
        }

        private bool OnResetCanExecute()
        {
            return CanReset;
        }

        #endregion

        protected void InitializeCommands()
        {
            RemoveFeed = new Command(OnRemoveFeedExecute);
            MoveUpFeed = new Command(OnMoveUpFeedExecute);
            MoveDownFeed = new Command(OnMoveDownFeedExecute);
            AddFeed = new Command(OnAddFeedExecute);
            Reset = new TaskCommand(OnResetExecuteAsync, OnResetCanExecute);
        }

        protected override async Task InitializeAsync()
        {
            ValidationTimer.Elapsed += OnValidationTimerElapsed;
            Feeds.CollectionChanged += OnFeedsCollectionChanged;
        }

        protected override async Task<bool> SaveAsync()
        {
            SaveFeeds();
            PackageSources = Feeds.ToList();
            return true;
        }

        protected override async Task CloseAsync()
        {
            ValidationTimer.Stop();
            Feeds.CollectionChanged -= OnFeedsCollectionChanged;
            ValidationTimer.Elapsed -= OnValidationTimerElapsed;
            Feeds.ForEach(f => UnsubscribeFromFeedPropertyChanged(f));
        }

        private static void StartValidationTimer()
        {
            if (ValidationTimer.Enabled)
            {
                ValidationTimer.Stop();
            }

            ValidationTimer.Start();
        }

        protected override void ValidateBusinessRules(List<IBusinessRuleValidationResult> validationResults)
        {
            if (SelectedFeed is null || !IsNamesNotUniqueRule(out var names))
            {
                return;
            }

            var results = names.Select(name => BusinessRuleValidationResult.CreateError($"Two or more feeds have same name '{name}'")).Cast<IBusinessRuleValidationResult>();
            validationResults.AddRange(results);
        }

        private void SaveFeeds()
        {
            SettingsFeeds.Clear();
            SettingsFeeds.AddRange(Feeds);

            _configurationService.SavePackageSources(Feeds);
            foreach(var feed in RemovedFeeds)
            {
                _configurationService.RemovePackageSource(feed);
            }
        }

        private bool IsNamesNotUniqueRule(out IEnumerable<string> invalidNames)
        {
            var names = new List<string>();

            var groups = Feeds.GroupBy(x => x.Name).Where(g => g.Count() > 1).ToList();

            groups.ForEach(g => names.Add(g.Key));

            invalidNames = names;

            return groups.Any();
        }

        private async Task VerifyFeedAsync(NuGetFeed feed)
        {
            if (feed is null)
            {
                return;
            }

            if (!feed.IsValid())
            {
                feed.VerificationResult = FeedVerificationResult.Invalid;
                return;
            }

            if (feed.IsLocal())
            {
                //should be truly checked?
                feed.VerificationResult = FeedVerificationResult.Valid;
                return;
            }

            feed.IsVerifiedNow = true;

            using (var cts = new CancellationTokenSource())
            {
                var result = await _feedVerificationService.VerifyFeedAsync(feed.Source, true, cts.Token);
                feed.VerificationResult = result;
            }

            feed.IsVerifiedNow = false;
        }

        private async void OnValidationTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (IsVerifying)
            {
                return;
            }

            IsVerifying = true;

            while (_validationQueue.Any())
            {
                var validationList = new List<NuGetFeed>();

                for (var i = 0; i < Math.Min(_validationQueue.Count, VerificationBatch); i++)
                {
                    validationList.Add(_validationQueue.Dequeue());
                }

                await Task.WhenAll(validationList.Select(x => VerifyFeedAsync(x)));
            }

            IsVerifying = false;
        }

        private void AddToValidationQueue(NuGetFeed feed)
        {
            if (_validationQueue.Contains(feed))
            {
                return;
            }

            _validationQueue.Enqueue(feed);
        }

        protected void OnFeedPropertyPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //run verification if source changed
            if (string.Equals(nameof(NuGetFeed.Source), e.PropertyName))
            {
                AddToValidationQueue(sender as NuGetFeed);
                StartValidationTimer();
            }

            //check business rule if any error expected
            if (HasErrors)
            {
                Validate(true);
            }
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (IsSaving)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(PackageSources)) && PackageSources is not null)
            {
                var storedPackageSources = PackageSources.OfType<NuGetFeed>().ToList();
                SettingsFeeds.AddRange(storedPackageSources);
                Feeds.AddRange(storedPackageSources);

                // Validate items on first initialization
                SupressFeedVerificationOnCollectionChanged = false;
                Feeds.ForEach(async x => await VerifyFeedAsync(x));
            }

            base.OnPropertyChanged(e);
        }

        private void OnFeedsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var removedFeeds = e.OldItems.OfType<NuGetFeed>().ToList();
                removedFeeds.ForEach(feed => UnsubscribeFromFeedPropertyChanged(feed));
            }

            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Move)
            {
                return;
            }

            if (e.NewItems is null)
            {
                return;
            }

            var newFeeds = e.NewItems.OfType<NuGetFeed>().ToList();

            SelectedFeed = newFeeds.LastOrDefault();

            foreach (var item in newFeeds)
            {
                SubscribeToFeedPropertyChanged(item);
                if (SupressFeedVerificationOnCollectionChanged)
                {
                    continue;
                }

                if (item.VerificationResult == FeedVerificationResult.Unknown)
                {
                    AddToValidationQueue(item);
                }
            }

            StartValidationTimer();
        }

        private void SubscribeToFeedPropertyChanged(NuGetFeed feed)
        {
            feed.PropertyChanged += OnFeedPropertyPropertyChanged;
        }

        private void UnsubscribeFromFeedPropertyChanged(NuGetFeed feed)
        {
            feed.PropertyChanged -= OnFeedPropertyPropertyChanged;
        }
    }
}
