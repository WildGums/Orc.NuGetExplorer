namespace Orc.NuGetExplorer.ViewModels;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Catel.Collections;
using Catel.IoC;
using Catel.Logging;
using Catel.MVVM;
using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using Orc.NuGetExplorer;
using Orc.NuGetExplorer.Cache;
using Orc.NuGetExplorer.Enums;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Pagination;
using Orc.NuGetExplorer.Providers;
using Orc.NuGetExplorer.Services;
using Orc.NuGetExplorer.Web;
using Timer = System.Timers.Timer;

internal class ExplorerPageViewModel : FeaturedViewModelBase, IManagerPage
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ExplorerPageViewModel));
    private static readonly int SingleTasksDelayMs = 800;
    private static readonly IHttpExceptionHandler<FatalProtocolException> PackageLoadingExceptionHandler = new FatalProtocolExceptionHandler();

    private static readonly Timer SingleDelayTimer = new(SingleTasksDelayMs);

#pragma warning disable IDE1006 // Naming Styles
    private static IDisposable? _context;
#pragma warning restore IDE1006 // Naming Styles
    private readonly IDefferedPackageLoaderService _deferredPackageLoaderService;
    private readonly IDispatcherService _dispatcherService;
    private readonly INuGetFeedVerificationService _nuGetFeedVerificationService;
    private readonly IPackageMetadataMediaDownloadService _packageMetadataMediaDownloadService;
    private readonly IPackageOperationContextService _packageOperationContextService;
    private readonly IRepositoryContextService _repositoryService;
    private readonly IPackageLoaderService _packagesLoaderService;

    private readonly INuGetCacheManager _nuGetCacheManager;
    private readonly INuGetConfigurationService _nuGetConfigurationService;
    private readonly IDispatcherProviderService _dispatcherProviderService;
    private readonly MetadataOrigin _pageType;

    private readonly PackageSearchParameters? _initialSearchParams;
    private readonly HashSet<CancellationTokenSource> _tokenSource = new();

    private ExplorerSettingsContainer _settings;

    public ExplorerPageViewModel(ExplorerPage page,
        IPackageLoaderService packagesLoaderService,
        IModelProvider<ExplorerSettingsContainer> settingsProvider,
        IPackageMetadataMediaDownloadService packageMetadataMediaDownloadService,
        INuGetFeedVerificationService nuGetFeedVerificationService,
        ICommandManager commandManager,
        IDispatcherService dispatcherService,
        IRepositoryContextService repositoryService,
        IServiceProvider serviceProvider,
        IDefferedPackageLoaderService deferredPackageLoaderService,
        IPackageOperationContextService packageOperationContextService,
        INuGetCacheManager nuGetCacheManager,
        INuGetConfigurationService nuGetConfigurationService,
        IDispatcherProviderService dispatcherProviderService)
        : base(serviceProvider)
    {
        _dispatcherService = dispatcherService;
        _packageMetadataMediaDownloadService = packageMetadataMediaDownloadService;
        _nuGetFeedVerificationService = nuGetFeedVerificationService;
        _repositoryService = repositoryService;
        _deferredPackageLoaderService = deferredPackageLoaderService;
        _packageOperationContextService = packageOperationContextService;
        _packagesLoaderService = packagesLoaderService;
        _nuGetCacheManager = nuGetCacheManager;
        _nuGetConfigurationService = nuGetConfigurationService;
        _dispatcherProviderService = dispatcherProviderService;

        if (settingsProvider.Model is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Settings must be initialized");
        }

        Settings = settingsProvider.Model;
        if (_settings is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Settings must be initialized");
        }

        LoadNextPackagePage = new TaskCommand(serviceProvider, LoadNextPackagePageExecuteAsync);
        CancelPageLoading = new TaskCommand(serviceProvider, CancelPageLoadingExecuteAsync);
        RefreshCurrentPage = new TaskCommand(serviceProvider, RefreshCurrentPageExecuteAsync);

        commandManager.RegisterCommand(nameof(RefreshCurrentPage), RefreshCurrentPage, this);

        Title = page.Parameters.Tab.Name;
        _initialSearchParams = page.Parameters.InitialSearchParameters; //if null, standard Settings will not be overriden

        if (Title != "Browse")
        {
            _packagesLoaderService = serviceProvider.GetRequiredKeyedService<IPackageLoaderService>(Title);
        }

        if (!Enum.TryParse(Title, out _pageType))
        {
            Logger.LogError("Unrecognized page type");
        }

        CanBatchProjectActions = _pageType != MetadataOrigin.Installed;

        PackageItems = new System.Collections.ObjectModel.ObservableCollection<NuGetPackage>();
        Page = page;
    }

    /// <summary>
    ///     Repository context.
    ///     Due to all pages uses package sources selected by user in settings
    ///     context is shared between pages too
    /// </summary>
    private static IDisposable? Context
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

    private PageContinuation? PageInfo { get; set; }

    private PageContinuation? AwaitedPageInfo { get; set; }

    private PackageSearchParameters? AwaitedSearchParameters { get; set; }

    public ExplorerSettingsContainer Settings
    {
        get { return _settings; }
        set
        {
            if (_settings != value)
            {
                if (_settings is not null)
                {
                    _settings.PropertyChanged -= OnSettingsPropertyPropertyChanged;
                }

                _settings = value;

                if (_settings is not null)
                {
                    _settings.PropertyChanged += OnSettingsPropertyPropertyChanged;
                }
            }
        }
    }


    // view to view model
    public NuGetPackage? SelectedPackageItem { get; set; }

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

    public bool CanBatchUpdateOperations => _pageType == MetadataOrigin.Updates;

    public bool CanBatchInstallOperations => _pageType == MetadataOrigin.Browse;

    public System.Collections.ObjectModel.ObservableCollection<NuGetPackage> PackageItems { get; set; }

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

    private void OnSettingsPropertyPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (Settings.ObservedFeed is null)
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

    private void OnPackageItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Add && e.NewStartingIndex == 0 && IsActive)
        {
            // Item added on first place, collection was empty
            SelectedPackageItem = PackageItems.FirstOrDefault();
        }
    }

    protected override async Task InitializeAsync()
    {
        try
        {
            if (IsActive && _initialSearchParams is not null)
            {
                // Set page initial search params as Settings parameters
                // Only on first loaded page
                Settings.IsPreReleaseIncluded = _initialSearchParams.IsPrereleaseIncluded;
                Settings.SearchString = _initialSearchParams.SearchString;
                Settings.IsRecommendedOnly = _initialSearchParams.IsRecommendedOnly;
            }

            // Execution delay
            SingleDelayTimer.Elapsed += OnTimerElapsed;
            SingleDelayTimer.AutoReset = false;

            //SingleDelayTimer.SynchronizingObject = ActivatorUtilities.CreateInstance<ISynchronizeInvoke>(ServiceProvider,
            //    _dispatcherProviderService.GetCurrentDispatcher());

            PackageItems.CollectionChanged += OnPackageItemsCollectionChanged;

            _packageOperationContextService.OperationContextDisposing += OnOperationContextDisposing;

            IsFirstLoaded = false;
            var pageSize = _nuGetConfigurationService.GetPackageQuerySize();

            if (Settings.ObservedFeed is not null && !string.IsNullOrEmpty(Settings.ObservedFeed.Source))
            {
                var currentFeed = Settings.ObservedFeed;
                PageInfo = new PageContinuation(pageSize, Settings.ObservedFeed.GetPackageSource());

                var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);

                await VerifySourceAndLoadPackagesAsync(PageInfo, currentFeed, searchParams, pageSize);
            }
            else
            {
                Logger.LogInformation("None of the source feeds configured");
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, null);
        }
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.HasPropertyChanged(nameof(Invalidated)))
        {
            Logger.LogInformation("ViewModel {ViewModel} {PropertyName} flag set to {Invalidated}", this, e.PropertyName, Invalidated);
        }

        if (e.HasPropertyChanged(nameof(IsActive)) && IsActive)
        {
            Logger.LogInformation("Switching page: {Title} is active", Title);

            // Force update selected item
            SelectedPackageItem = PackageItems.FirstOrDefault();
        }

        if (IsFirstLoaded)
        {
            return;
        }

        if (e.HasPropertyChanged(nameof(IsActive)) && Invalidated)
        {
            // Just switching page, no need to invalidate data
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

    private static void StartLoadingTimer()
    {
        if (SingleDelayTimer.Enabled)
        {
            SingleDelayTimer.Stop();
        }

        SingleDelayTimer.Start();

        Logger.LogDebug("Start loading delay timer");
    }

    private async void OnTimerElapsed(object? sender, ElapsedEventArgs e)
    {
        var currentFeed = Settings.ObservedFeed;
        if (currentFeed is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Cannot perform update on empty feed");
        }

        Logger.LogInformation("Updating page from feed {FeedName}", currentFeed.Name);

        // Reset page package data
        PageInfo = new PageContinuation(_nuGetConfigurationService.GetPackageQuerySize(), currentFeed.GetPackageSource());

        var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);
        var pageSize = _nuGetConfigurationService.GetPackageQuerySize();
        await VerifySourceAndLoadPackagesAsync(PageInfo, currentFeed, searchParams, pageSize);
    }

    private async Task VerifySourceAndLoadPackagesAsync(PageContinuation pageinfo, INuGetSource currentSource, PackageSearchParameters? searchParams, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(pageinfo);
        ArgumentNullException.ThrowIfNull(currentSource);

        try
        {
            if (pageinfo.Source.IsMultipleSource)
            {
                Context = _repositoryService.AcquireContext();
            }
            else
            {
                var source = (PackageSource)pageinfo.Source;
                Context = _repositoryService.AcquireContext(source);
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
                Logger.LogDebug("You can now cancel search from gui");

                using (var pageTcs = GetCancelationTokenSource())
                {
                    if (!currentSource.IsVerified)
                    {
                        await CanFeedBeLoadedAsync(currentSource, VerificationTokenSource.Token);
                    }

                    if (!currentSource.IsAccessible && _pageType != MetadataOrigin.Installed)
                    {
                        IsCancellationTokenAlive = false;
                        return;
                    }

                    if (!IsLoadingInProcess)
                    {
                        await LoadPackagesAsync(pageinfo, searchParams ?? new(), pageTcs.Token);
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

                    IsCancellationTokenAlive = false;
                }
            }
        }
        catch (OperationCanceledException ex)
        {
            Logger.LogInformation(ex, "Command {CommandName} was cancelled", nameof(LoadPackagesAsync));

            IsCancellationTokenAlive = false;

            // backward page if needed
            if (PageInfo?.LastNumber > pageSize)
            {
                PageInfo.GetPrevious();
            }

            // restart
            if (AwaitedPageInfo is not null)
            {
                var awaitedPageinfo = AwaitedPageInfo;
                var awaitedSeachParams = AwaitedSearchParameters;
                AwaitedPageInfo = null;
                AwaitedSearchParameters = null;

                await VerifySourceAndLoadPackagesAsync(awaitedPageinfo, Settings?.ObservedFeed ?? throw new InvalidOperationException("Must be non-empty field"), awaitedSeachParams, pageSize);
            }
            else
            {
                Logger.LogInformation("Search operation was canceled (interrupted by next user request");
            }
        }
        catch (FatalProtocolException ex)
        {
            IsCancellationTokenAlive = false;
            var result = PackageLoadingExceptionHandler.HandleException(ex, currentSource.Source);

            if (result == FeedVerificationResult.AuthenticationRequired)
            {
                Logger.LogError("Authentication credentials required. Cannot load packages from source '{Source}'", currentSource.Source);
            }
            else
            {
                Logger.LogError(ex, null);
            }
        }
        catch (Exception ex)
        {
            IsCancellationTokenAlive = false;
            Logger.LogError(ex, null);
        }
        finally
        {
            if (PackageItems.Any() && IsActive)
            {
                await _deferredPackageLoaderService.StartLoadingAsync();
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

    private async Task LoadPackagesAsync(PageContinuation pageInfo, PackageSearchParameters searchParameters, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageInfo);
        ArgumentNullException.ThrowIfNull(searchParameters);

        try
        {
            IsLoadingInProcess = true;

            Logger.LogInformation("Start package query on {Title} page", Title);

            var isFirstLoad = pageInfo.Current < 0;

            if (isFirstLoad)
            {
                PackageItems.Clear();
            }

            IEnumerable<IPackageSearchMetadata>? packages = null;

            if (searchParameters.IsRecommendedOnly && _packagesLoaderService is IPackagesUpdatesSearcherService updatesLoaderService)
            {
                Logger.LogInformation("Select only recommended upgrades");
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

            Logger.LogInformation("Page '{Title}' updated with {PackageCount} packages returned by query from {Source}'", Title, packages.Count(), PageInfo?.Source);
        }
        finally
        {
            IsLoadingInProcess = false;
        }
    }

    private async Task CreatePackageListItemsAsync(IEnumerable<IPackageSearchMetadata> packageSearchMetadataCollection)
    {
        ArgumentNullException.ThrowIfNull(packageSearchMetadataCollection);

        var models = packageSearchMetadataCollection.Select(x => ActivatorUtilities.CreateInstance<NuGetPackage>(ServiceProvider, x, _pageType)).ToList();

        //create tokens, used for defer execution of tasks
        //obtained states/updates of packages

        if (_pageType != MetadataOrigin.Updates)
        {
            foreach (var package in models)
            {
                var deferToken = new DeferToken(_pageType, package)
                {
                    UpdateAction = newState =>
                    {
                        package.Status = newState;
                    }
                };

                if (_repositoryService.AcquireContext() != SourceContext.EmptyContext)
                {
                    _deferredPackageLoaderService.Add(deferToken);
                }
            }
        }

        _dispatcherService.Invoke(() =>
        {
            PackageItems.AddRange(models);
        });
    }

    private async Task DownloadAllPicturesForMetadataAsync(IEnumerable<IPackageSearchMetadata> metadatas, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(metadatas);

        foreach (var metadata in metadatas)
        {
            if (metadata.IconUrl is not null)
            {
                token.ThrowIfCancellationRequested();
                await _packageMetadataMediaDownloadService.DownloadMediaForMetadataAsync(metadata);
            }
        }

        await Task.CompletedTask;
    }

    private void OnOperationContextDisposing(object? sender, OperationContextEventArgs e)
    {
        StartLoadingTimerOrInvalidateData();
    }

    private async Task CanFeedBeLoadedAsync(INuGetSource source, CancellationToken cancelToken)
    {
        Logger.LogInformation("'{Source}' package source is verified", source);

        if (source is NuGetFeed singleSource)
        {
            singleSource.VerificationResult = singleSource.IsLocal()
                ? FeedVerificationResult.Valid
                : await _nuGetFeedVerificationService.VerifyFeedAsync(source.Source, cancellationToken: cancelToken);
        }
        else if (source is CombinedNuGetSource combinedSource)
        {
            var inaccessibleFeeds = new List<NuGetFeed>();

            foreach (var feed in combinedSource.GetAllSources())
            {
                feed.VerificationResult = feed.IsLocal()
                    ? FeedVerificationResult.Valid
                    : await _nuGetFeedVerificationService.VerifyFeedAsync(feed.Source, cancellationToken: cancelToken);

                if (!feed.IsAccessible)
                {
                    inaccessibleFeeds.Add(feed);
                    Logger.LogWarning("{Feed} is inaccessible. It won't be used when 'All' option selected", feed);
                }
            }

            inaccessibleFeeds.ForEach(x => combinedSource.RemoveFeed(x));
        }
        else
        {
            Logger.LogError("Parameter {Source} has invalid type", source);
        }
    }

    #region commands
    public TaskCommand LoadNextPackagePage { get; set; }

    private async Task LoadNextPackagePageExecuteAsync()
    {
        await LoadNextPackagePageExecuteAsync(Settings.ObservedFeed, PageInfo);
    }

    private async Task LoadNextPackagePageExecuteAsync(INuGetSource? pageSource, PageContinuation? pageToken)
    {
        ArgumentNullException.ThrowIfNull(pageSource);
        ArgumentNullException.ThrowIfNull(pageToken);

        var pageSize = _nuGetConfigurationService.GetPackageQuerySize();
        var searchParams = new PackageSearchParameters(Settings.IsPreReleaseIncluded, Settings.SearchString, Settings.IsRecommendedOnly);
        var currentFeed = Settings.ObservedFeed;
        if (currentFeed is null)
        {
            throw Logger.LogErrorAndCreateException<InvalidOperationException>("Could not load NuGet packages from empty feed");
        }

        await VerifySourceAndLoadPackagesAsync(pageToken, currentFeed, searchParams, pageSize);
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
                await token.CancelAsync();
            }
        }

        IsCancellationForced = false;
    }

    public TaskCommand RefreshCurrentPage { get; set; }

    private async Task RefreshCurrentPageExecuteAsync()
    {
        _nuGetCacheManager.ClearHttpCache();

        StartLoadingTimerOrInvalidateData();
    }
    #endregion
}
