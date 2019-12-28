namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
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
    using Catel.Services;
    using Catel.Windows.Threading;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer;
    using Orc.NuGetExplorer.Enums;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Models;
    using Orc.NuGetExplorer.Pagination;
    using Orc.NuGetExplorer.Providers;
    using Orc.NuGetExplorer.Services;
    using Orc.NuGetExplorer.Web;
    using Timer = System.Timers.Timer;

    internal class ExplorerPageViewModel : ViewModelBase, IManagerPage
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly int PageSize = 17;
        private static readonly int SingleTasksDelayMs = 800;
        private static readonly IHttpExceptionHandler<FatalProtocolException> PackageLoadingExceptionHandler = new FatalProtocolExceptionHandler();

        private static readonly Timer SingleDelayTimer = new Timer(SingleTasksDelayMs);

#pragma warning disable IDE1006 // Naming Styles
        private static IDisposable _context;
#pragma warning restore IDE1006 // Naming Styles
        private readonly IDefferedPackageLoaderService _defferedPackageLoaderService;
        private readonly IDispatcherService _dispatcherService;
        private readonly INuGetFeedVerificationService _nuGetFeedVerificationService;
        private readonly IPackageMetadataMediaDownloadService _packageMetadataMediaDownloadService;

        private readonly INuGetPackageManager _projectManager;
        private readonly IPackageOperationContextService _packageOperationContextService;
        private readonly IRepositoryContextService _repositoryService;

        private readonly HashSet<CancellationTokenSource> _tokenSource = new HashSet<CancellationTokenSource>();
        private readonly ITypeFactory _typeFactory;

        private readonly PackageSearchParameters _initialSearchParams;
        private readonly IPackageLoaderService _packagesLoaderService;
        private readonly MetadataOrigin _pageType;
        private ExplorerSettingsContainer _settings;

        public ExplorerPageViewModel(ExplorerPage page, IPackageLoaderService packagesLoaderService,
            IModelProvider<ExplorerSettingsContainer> settingsProvider, IPackageMetadataMediaDownloadService packageMetadataMediaDownloadService, INuGetFeedVerificationService nuGetFeedVerificationService,
            ICommandManager commandManager, IDispatcherService dispatcherService, IRepositoryContextService repositoryService, ITypeFactory typeFactory,
            IDefferedPackageLoaderService defferedPackageLoaderService, INuGetPackageManager projectManager, IPackageOperationContextService packageOperationContextService)
        {
            Argument.IsNotNull(() => packagesLoaderService);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => packageMetadataMediaDownloadService);
            Argument.IsNotNull(() => commandManager);
            Argument.IsNotNull(() => nuGetFeedVerificationService);
            Argument.IsNotNull(() => dispatcherService);
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => typeFactory);
            Argument.IsNotNull(() => defferedPackageLoaderService);
            Argument.IsNotNull(() => projectManager);
            Argument.IsNotNull(() => packageOperationContextService);

            _dispatcherService = dispatcherService;
            _packageMetadataMediaDownloadService = packageMetadataMediaDownloadService;
            _nuGetFeedVerificationService = nuGetFeedVerificationService;
            _repositoryService = repositoryService;
            _defferedPackageLoaderService = defferedPackageLoaderService;
            _projectManager = projectManager;
            _packageOperationContextService = packageOperationContextService;
            _typeFactory = typeFactory;

            _packagesLoaderService = packagesLoaderService;

            Settings = settingsProvider.Model;

            LoadNextPackagePage = new TaskCommand(LoadNextPackagePageExecuteAsync);
            CancelPageLoading = new TaskCommand(CancelPageLoadingExecuteAsync);
            RefreshCurrentPage = new TaskCommand(RefreshCurrentPageExecuteAsync);

            commandManager.RegisterCommand(nameof(RefreshCurrentPage), RefreshCurrentPage, this);

            Title = page.Parameters.Tab.Name;
            _initialSearchParams = page.Parameters.InitialSearchParameters; //if null, standard Settings will not be overriden

            if (Title != "Browse")
            {
                _packagesLoaderService = this.GetServiceLocator().ResolveType<IPackageLoaderService>(Title);
            }

            if (!Enum.TryParse(Title, out _pageType))
            {
                Log.Error("Unrecognized page type");
            }

            CanBatchProjectActions = _pageType == MetadataOrigin.Updates;

            Page = page;
        }

        /// <summary>
        ///     Repository context.
        ///     Due to all pages uses package sources selected by user in settings
        ///     context is shared between pages too
        /// </summary>
        private static IDisposable Context
        {
            get { return _context; }
            set
            {
                if (_context != value)
                {
                    _context?.Dispose();
                    _context = value;
                }
            }
        }

        public static CancellationTokenSource VerificationTokenSource { get; set; } = new CancellationTokenSource();

        public static CancellationTokenSource DelayCancellationTokenSource { get; set; } = new CancellationTokenSource();

        private PageContinuation PageInfo { get; set; }

        private PageContinuation AwaitedPageInfo { get; set; }

        private PackageSearchParameters AwaitedSearchParameters { get; set; }

        public ExplorerSettingsContainer Settings
        {
            get { return _settings; }
            set
            {
                if (_settings != null)
                {
                    _settings.PropertyChanged -= OnSettingsPropertyPropertyChanged;
                }

                _settings = value;

                if (_settings != null)
                {
                    _settings.PropertyChanged += OnSettingsPropertyPropertyChanged;
                }
            }
        }


        //view to view model
        public NuGetPackage SelectedPackageItem { get; set; }

        //view to viewmodel
        [Model(SupportIEditableObject = false)]
        public ExplorerPage Page { get; set; }

        [ViewModelToModel]
        public bool IsActive { get; set; }

        /// <summary>
        ///     Shows is data should be reloaded
        ///     when viewmodel became active
        /// </summary>
        public bool Invalidated { get; set; }

        public bool IsCancellationTokenAlive { get; set; }

        public bool IsLoadingInProcess { get; set; }

        public bool IsFirstLoaded { get; set; } = true;

        public bool IsCancellationForced { get; set; }

        /// <summary>
        ///     Is project manipulations can be performed on multiple packages
        ///     on this page in one operation
        /// </summary>
        public bool CanBatchProjectActions { get; set; }

        public CancellationTokenSource PageLoadingTokenSource { get; set; }

        public FastObservableCollection<NuGetPackage> PackageItems { get; set; }

        public void StartLoadingTimerOrInvalidateData()
        {
            if (IsActive)
            {
                StartLoadingTimer();
            }
            else
            {
                Invalidated = true;
            }
        }

        private void OnSettingsPropertyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Settings.ObservedFeed == null)
            {
                return;
            }

            if (IsFirstLoaded)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(Settings.IsPreReleaseIncluded)) ||
                string.Equals(e.PropertyName, nameof(Settings.SearchString)) || string.Equals(e.PropertyName, nameof(Settings.ObservedFeed)))
            {
                StartLoadingTimerOrInvalidateData();
            }
        }

        private void OnPackageItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //item added on first place, collection was empty
                if (e.NewStartingIndex == 0 && IsActive)
                {
                    SelectedPackageItem = PackageItems.FirstOrDefault();
                }
            }
        }

        protected override async Task InitializeAsync()
        {
            try
            {
                if (IsActive && _initialSearchParams != null)
                {
                    //set page initial search params as Settings parameters
                    //only on first loaded page
                    Settings.IsPreReleaseIncluded = _initialSearchParams.IsPrereleaseIncluded;
                    Settings.SearchString = _initialSearchParams.SearchString;
                    Settings.IsRecommendedOnly = _initialSearchParams.IsRecommendedOnly;
                }

                //execution delay
                SingleDelayTimer.Elapsed += OnTimerElapsed;
                SingleDelayTimer.AutoReset = false;

                SingleDelayTimer.SynchronizingObject = _typeFactory.CreateInstanceWithParameters<ISynchronizeInvoke>(DispatcherHelper.CurrentDispatcher);

                PackageItems = new FastObservableCollection<NuGetPackage>();

                PackageItems.CollectionChanged += OnPackageItemsCollectionChanged;

                _packageOperationContextService.OperationContextDisposing += OnOperationContextDisposing;

                IsFirstLoaded = false;

                if (Settings.ObservedFeed != null && !string.IsNullOrEmpty(Settings.ObservedFeed.Source))
                {
                    var currentFeed = Settings.ObservedFeed;
                    PageInfo = new PageContinuation(PageSize, Settings.ObservedFeed.GetPackageSource());

                    var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);

                    await VerifySourceAndLoadPackagesAsync(PageInfo, currentFeed, searchParams);
                }
                else
                {
                    Log.Info("None of the source feeds configured");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (string.Equals(e.PropertyName, nameof(Invalidated)))
            {
                Log.Info($"ViewModel {this} {e.PropertyName} flag set to {Invalidated}");
            }

            if (string.Equals(e.PropertyName, nameof(IsActive)))
            {
                if ((bool)e.NewValue)
                {
                    Log.Info($"Switching page: {Title} is active");

                    //force update selected item
                    SelectedPackageItem = PackageItems?.FirstOrDefault();
                }
            }

            if (IsFirstLoaded)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(IsActive)) && Invalidated)
            {
                //just switching page, no need to invalidate data
                StartLoadingTimer();
            }
        }

        protected override async Task OnClosedAsync(bool? result)
        {
            PackageItems.CollectionChanged -= OnPackageItemsCollectionChanged;
            Settings.PropertyChanged -= OnSettingsPropertyPropertyChanged;
            SingleDelayTimer.Elapsed -= OnTimerElapsed;
            _packageOperationContextService.OperationContextDisposing -= OnOperationContextDisposing;
        }

        private void StartLoadingTimer()
        {
            if (SingleDelayTimer.Enabled)
            {
                SingleDelayTimer.Stop();
            }

            SingleDelayTimer.Start();

            Log.Debug("Start loading delay timer");
        }

        private async void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Log.Info("Timer elapsed");
            var currentFeed = Settings.ObservedFeed;
            //reset page
            PageInfo = new PageContinuation(PageSize, currentFeed.GetPackageSource());

            var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);
            await VerifySourceAndLoadPackagesAsync(PageInfo, currentFeed, searchParams);
        }

        private async Task VerifySourceAndLoadPackagesAsync(PageContinuation pageinfo, INuGetSource currentSource, PackageSearchParameters searchParams)
        {
            try
            {
                if (pageinfo.Source.IsMultipleSource)
                {
                    Context = _repositoryService.AcquireContext();
                }
                else
                {
                    Context = _repositoryService.AcquireContext((PackageSource)pageinfo.Source);
                }


                if (IsActive)
                {
                    if (Context == SourceContext.EmptyContext && _pageType == MetadataOrigin.Browse)
                    {
                        //clear all packages, context does not contains any repos
                        PackageItems.Clear();
                    }

                    if (Context == SourceContext.EmptyContext && _pageType == MetadataOrigin.Installed)
                    {
                        //create valid page continuation for only-local query
                        pageinfo = new PageContinuation(pageinfo, true);
                    }

                    IsCancellationTokenAlive = true;
                    Log.Debug("You can now cancel search from gui");

                    using (var pageTcs = GetCancelationTokenSource())
                    {
                        if (!currentSource.IsVerified)
                        {
                            await CanFeedBeLoadedAsync(VerificationTokenSource.Token, currentSource);
                        }

                        if (!currentSource.IsAccessible && _pageType != MetadataOrigin.Installed)
                        {
                            IsCancellationTokenAlive = false;
                            return;
                        }

                        if (!IsLoadingInProcess)
                        {
                            await LoadPackagesAsync(pageinfo, pageTcs.Token, searchParams);
                        }
                        else
                        {
                            if (IsCancellationForced)
                            {
                                //to prevent load restarting if cancellation initiated by user
                                AwaitedPageInfo = null;
                            }
                            else
                            {
                                AwaitedPageInfo = PageInfo;
                                AwaitedSearchParameters = searchParams;
                            }

                            //task with pageTcs source cancel all loading tasks in-process
                            CancelLoadingTasks(pageTcs);
                        }

                        _tokenSource.Remove(pageTcs);

                        PageLoadingTokenSource = null;
                        IsCancellationTokenAlive = false;
                    }
                }
            }
            catch (OperationCanceledException ex)
            {
                Log.Info($"Command {nameof(LoadPackagesAsync)} was cancelled by {ex}");

                IsCancellationTokenAlive = false;

                //backward page if needed
                if (PageInfo.LastNumber > PageSize)
                {
                    PageInfo.GetPrevious();
                }

                //restart
                if (AwaitedPageInfo != null)
                {
                    var awaitedPageinfo = AwaitedPageInfo;
                    var awaitedSeachParams = AwaitedSearchParameters;
                    AwaitedPageInfo = null;
                    AwaitedSearchParameters = null;
                    await VerifySourceAndLoadPackagesAsync(awaitedPageinfo, Settings.ObservedFeed, awaitedSeachParams);
                }
                else
                {
                    Log.Info("Search operation was canceled (interrupted by next user request");
                }
            }
            catch (FatalProtocolException ex)
            {
                IsCancellationTokenAlive = false;
                var result = PackageLoadingExceptionHandler.HandleException(ex, currentSource.Source);

                if (result == FeedVerificationResult.AuthenticationRequired)
                {
                    Log.Error($"Authentication credentials required. Cannot load packages from source '{currentSource.Source}'");
                }
                else
                {
                    Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                IsCancellationTokenAlive = false;
                Log.Error(ex);
            }
            finally
            {
                if (PackageItems.Any() && IsActive)
                {
                    await _defferedPackageLoaderService.StartLoadingAsync();
                }
            }
        }

        private CancellationTokenSource GetCancelationTokenSource()
        {
            var source = new CancellationTokenSource();

            _tokenSource.Add(source);

            return source;
        }

        private void CancelLoadingTasks(CancellationTokenSource token)
        {
            foreach (var tokenSource in _tokenSource)
            {
                if (tokenSource != token)
                {
                    tokenSource.Cancel();
                }
            }
        }

        private async Task LoadPackagesAsync(PageContinuation pageInfo, CancellationToken cancellationToken, PackageSearchParameters searchParameters)
        {
            try
            {
                IsLoadingInProcess = true;

                Log.Info($"Start package query on {Title} page");

                var isFirstLoad = pageInfo.Current < 0;

                if (isFirstLoad)
                {
                    PackageItems.Clear();
                }

                IEnumerable<IPackageSearchMetadata> packages = null;

                if (searchParameters.IsRecommendedOnly && _packagesLoaderService is IPackagesUpdatesSearcherService updatesLoaderService)
                {
                    Log.Info("Select only recommended upgrades");
                    packages = await updatesLoaderService.SearchForPackagesUpdatesAsync(token: cancellationToken);
                }
                else
                {
                    packages = await _packagesLoaderService.LoadAsync(
                    searchParameters.SearchString, pageInfo, new SearchFilter(searchParameters.IsPrereleaseIncluded), cancellationToken);
                }


                await DownloadAllPicturesForMetadataAsync(packages, cancellationToken);

                cancellationToken.ThrowIfCancellationRequested();

                await CreatePackageListItemsAsync(packages);

                Invalidated = false;

                Log.Info($"Page '{Title}' updated with {packages.Count()} packages returned by query from {PageInfo.Source}'");
            }
            finally
            {
                IsLoadingInProcess = false;
            }
        }

        private async Task CreatePackageListItemsAsync(IEnumerable<IPackageSearchMetadata> packageSearchMetadataCollection)
        {
            var vms = packageSearchMetadataCollection.Select(x => _typeFactory.CreateInstanceWithParametersAndAutoCompletion<NuGetPackage>(x, _pageType)).ToList();

            //create tokens, used for deffer execution of tasks
            //obtained states/updates of packages

            if (_pageType != MetadataOrigin.Updates)
            {
                foreach (var vm in vms)
                {
                    var deferToken = new DeferToken();

                    deferToken.LoadType = DetermineLoadBehavior(_pageType);
                    deferToken.Package = vm;

                    deferToken.UpdateAction = newState =>
                    {
                        vm.Status = newState;
                    };

                    if (_repositoryService.AcquireContext() != SourceContext.EmptyContext)
                    {
                        _defferedPackageLoaderService.Add(deferToken);
                    }
                }
            }

            _dispatcherService.Invoke(() =>
            {
                PackageItems.AddRange(vms);
            }
            );

            MetadataOrigin DetermineLoadBehavior(MetadataOrigin page)
            {
                switch (page)
                {
                    case MetadataOrigin.Browse: return MetadataOrigin.Installed;

                    case MetadataOrigin.Installed: return MetadataOrigin.Browse;
                }

                return MetadataOrigin.Browse;
            }
        }

        private async Task DownloadAllPicturesForMetadataAsync(IEnumerable<IPackageSearchMetadata> metadatas, CancellationToken token)
        {
            foreach (var metadata in metadatas)
            {
                if (metadata.IconUrl != null)
                {
                    token.ThrowIfCancellationRequested();
                    await _packageMetadataMediaDownloadService.DownloadMediaForMetadataAsync(metadata);
                }
            }

            await Task.CompletedTask;
        }

        private void OnOperationContextDisposing(object sender, OperationContextEventArgs e)
        {
            StartLoadingTimerOrInvalidateData();
        }

        private async Task CanFeedBeLoadedAsync(CancellationToken cancelToken, INuGetSource source)
        {
            Log.Info($"'{source}' package source is verified");

            if (source is NuGetFeed)
            {
                var singleSource = source as NuGetFeed;

                singleSource.VerificationResult = singleSource.IsLocal()
                    ? FeedVerificationResult.Valid
                    : await _nuGetFeedVerificationService.VerifyFeedAsync(source.Source, cancellationToken: cancelToken);
            }
            else if (source is CombinedNuGetSource)
            {
                var combinedSource = source as CombinedNuGetSource;
                var unaccessibleFeeds = new List<NuGetFeed>();

                foreach (var feed in combinedSource.GetAllSources())
                {
                    feed.VerificationResult = feed.IsLocal()
                        ? FeedVerificationResult.Valid
                        : await _nuGetFeedVerificationService.VerifyFeedAsync(feed.Source, cancellationToken: cancelToken);

                    if (!feed.IsAccessible)
                    {
                        unaccessibleFeeds.Add(feed);
                        Log.Warning($"{feed} is unaccessible. It won't be used when 'All' option selected");
                    }
                }

                unaccessibleFeeds.ForEach(x => combinedSource.RemoveFeed(x));
            }
            else
            {
                Log.Error($"Parameter {source} has invalid type");
            }
        }

        #region commands
        public TaskCommand LoadNextPackagePage { get; set; }

        private async Task LoadNextPackagePageExecuteAsync()
        {
            var pageInfo = PageInfo;
            var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);
            await VerifySourceAndLoadPackagesAsync(pageInfo, Settings.ObservedFeed, searchParams);
        }

        public TaskCommand CancelPageLoading { get; set; }

        private async Task CancelPageLoadingExecuteAsync()
        {
            IsCancellationForced = true;

            //force cancel all operations
            if (IsCancellationTokenAlive)
            {
                foreach (var token in _tokenSource)
                {
                    token.Cancel();
                }
            }

            IsCancellationForced = false;
        }

        public TaskCommand RefreshCurrentPage { get; set; }

        private async Task RefreshCurrentPageExecuteAsync()
        {
            StartLoadingTimerOrInvalidateData();
        }
        #endregion
    }
}
