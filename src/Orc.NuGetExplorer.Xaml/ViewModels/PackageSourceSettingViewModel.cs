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
    using Catel.IoC;
    using Catel.Logging;
    using Catel.MVVM;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Providers;

    internal class PackageSourceSettingViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly int ValidationDelay = 800;
        private static readonly int VerificationBatch = 5;

        private readonly IServiceLocator _serviceLocator;
        private readonly INuGetConfigurationService _configurationService;
        private readonly INuGetFeedVerificationService _feedVerificationService;

        private readonly Queue<NuGetFeed> _validationQueue = new Queue<NuGetFeed>();

        private static readonly System.Timers.Timer ValidationTimer = new System.Timers.Timer(ValidationDelay);

        private INuGetConfigurationResetService _nuGetConfigurationResetService;

        public PackageSourceSettingViewModel(List<NuGetFeed> configuredFeeds, INuGetConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService, IServiceLocator serviceLocator) 
            : this(configurationService, feedVerificationService, serviceLocator)
        {
            Argument.IsNotNull(() => configuredFeeds);

            SettingsFeeds = configuredFeeds ?? new List<NuGetFeed>();
            Feeds.AddRange(SettingsFeeds);
        }

        public PackageSourceSettingViewModel(INuGetConfigurationService configurationService, INuGetFeedVerificationService feedVerificationService,
            IServiceLocator serviceLocator)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => feedVerificationService);
            Argument.IsNotNull(() => serviceLocator);

            _configurationService = configurationService;
            _feedVerificationService = feedVerificationService;
            _serviceLocator = serviceLocator;

            RemovedFeeds = new List<NuGetFeed>();

            DeferValidationUntilFirstSaveCall = true;

            DefaultFeed = Constants.DefaultNuGetOrgUri;
            DefaultSourceName = Constants.DefaultNuGetOrgName;

            SettingsFeeds = new List<NuGetFeed>();
            Feeds = new ObservableCollection<NuGetFeed>();

            ValidationTimer.Elapsed += OnValidationTimerElapsed;

            CommandInitialize();
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

        private bool ListenViewToViewModelPropertyChanges { get; set; } = true;
        private bool SupressCollectionChanged { get; set; } = true;

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
            await _nuGetConfigurationResetService.Reset();
        }

        private bool OnResetCanExecute()
        {
            return _nuGetConfigurationResetService != null;
        }

        #endregion

        protected void CommandInitialize()
        {
            RemoveFeed = new Command(OnRemoveFeedExecute);
            MoveUpFeed = new Command(OnMoveUpFeedExecute);
            MoveDownFeed = new Command(OnMoveDownFeedExecute);
            AddFeed = new Command(OnAddFeedExecute);
            Reset = new TaskCommand(OnResetExecuteAsync, OnResetCanExecute);
        }

        protected override async Task InitializeAsync()
        {
            Title = "Settings";

            Feeds.CollectionChanged += OnFeedsCollectionChanged;
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
            Feeds.CollectionChanged -= OnFeedsCollectionChanged;
            Feeds.ForEach(f => UnsubscribeFromFeedPropertyChanged(f));

            return base.CloseAsync();
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
            if (feed == null || !feed.IsValid())
            {
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

        private void StartValidationTimer()
        {
            if (ValidationTimer.Enabled)
            {
                ValidationTimer.Stop();
            }

            ValidationTimer.Start();
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

                for(int i = 0; i < Math.Min(_validationQueue.Count, VerificationBatch); i++)
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
                StartValidationTimer();
            }
        }

        private void OnResetChanged()
        {
            if(CanReset)
            {
                _nuGetConfigurationResetService = _serviceLocator.ResolveType<INuGetConfigurationResetService>();
            }
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

                //validate items on first initialization
                SupressCollectionChanged = false;
                Feeds.ForEach(async x => await VerifyFeedAsync(x));

            }

            base.OnPropertyChanged(e);
        }

        private void OnFeedsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            //verify all new feeds in collection
            if(SupressCollectionChanged)
            {
                return;
            }

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

            foreach (NuGetFeed item in newFeeds)
            {
                SubscribeToFeedPropertyChanged(item);

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
