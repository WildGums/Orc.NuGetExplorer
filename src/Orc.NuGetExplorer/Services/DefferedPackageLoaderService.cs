﻿namespace Orc.NuGetExplorer.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGetExplorer.Enums;
using NuGetExplorer.Management;
using NuGetExplorer.Packaging;
using NuGetExplorer.Pagination;
using NuGetExplorer.Providers;

internal class DefferedPackageLoaderService : IDefferedPackageLoaderService
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private readonly IList<DeferToken> _taskTokenList = new List<DeferToken>();

    private CancellationToken _aliveCancellationToken;

    private bool _isLoading = false;

    private readonly IRepositoryContextService _repositoryService;
    private readonly INuGetPackageManager _projectManager;
    private readonly IModelProvider<ExplorerSettingsContainer> _settingsProvider;
    private readonly IDefaultExtensibleProjectProvider _projectProvider;

    private IPackageMetadataProvider? _packageMetadataProvider;

    public DefferedPackageLoaderService(IRepositoryContextService repositoryService, INuGetPackageManager nuGetExtensibleProjectManager,
        IModelProvider<ExplorerSettingsContainer> settingsProvider, IDefaultExtensibleProjectProvider projectProvider)
    {
        ArgumentNullException.ThrowIfNull(repositoryService);
        ArgumentNullException.ThrowIfNull(nuGetExtensibleProjectManager);
        ArgumentNullException.ThrowIfNull(settingsProvider);
        ArgumentNullException.ThrowIfNull(projectProvider);

        _repositoryService = repositoryService;
        _projectManager = nuGetExtensibleProjectManager;
        _settingsProvider = settingsProvider;
        _projectProvider = projectProvider;
    }

    public async Task StartLoadingAsync()
    {
        if (_isLoading)
        {
            return;
        }

        await RunTaskExecutionLoopAsync();
    }

    private async Task RunTaskExecutionLoopAsync()
    {
        try
        {
            var processedTask = _taskTokenList.ToList();
            _taskTokenList.Clear();

            _packageMetadataProvider = InitializeMetadataProvider();

            if (_packageMetadataProvider is null)
            {
                Log.Info("Cannot acquire metadata provider for background loading tasks");
                return;
            }

            using (var cts = new CancellationTokenSource())
            {
                _aliveCancellationToken = cts.Token;

#pragma warning disable IDISP013 // Await in using.
                var taskList = processedTask.ToDictionary(x => CreateTaskFromToken(x, _aliveCancellationToken));
#pragma warning restore IDISP013 // Await in using.

                Log.Info($"Start updating {_taskTokenList.Count} items in background");

                while (taskList.Any())
                {
                    var nextCompletedTask = await Task.WhenAny(taskList.Keys);

                    PackageStatus updateStateValue;
                    IPackageSearchMetadata? result = null;
                    DeferToken? executedToken = null;

                    try
                    {
                        executedToken = await nextCompletedTask;
                        result = executedToken?.Result;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Package loading background task failed, cannot get result");
                    }
                    finally
                    {
                        taskList.Remove(nextCompletedTask);
                    }

                    if (result is not null)
                    {
                        if (executedToken is null)
                        {
                            throw Log.ErrorAndCreateException<InvalidOperationException>($"Unexpected null value: \"{nameof(executedToken)}\"");
                        }

                        updateStateValue = await NuGetPackageCombinator.CombineAsync(executedToken.Package, executedToken.LoadType, result);
                    }
                    else
                    {
                        updateStateValue = PackageStatus.NotInstalled;
                    }

                    var updateAction = executedToken?.UpdateAction;
                    updateAction?.Invoke(updateStateValue);
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Background loading task was failed");
        }
        finally
        {
            //todo try to complete all remaining tasks
            _isLoading = false;
        }
    }
#pragma warning disable CL0002 // Use async suffix
    private Task<DeferToken> CreateTaskFromToken(DeferToken token, CancellationToken cancellationToken)
    {
        if (token.LoadType == MetadataOrigin.Installed)
        {
            //from local
            return GetMetadataFromLocalSourcesAsync(token, cancellationToken);
        }

        return GetMetadataFromRemoteSourcesAsync(token, cancellationToken);
    }
#pragma warning restore CL0002 // Use async suffix

    public IPackageMetadataProvider? InitializeMetadataProvider()
    {

        var typeFactory = TypeFactory.Default;
        //todo provide more automatic way
        //create package metadata provider from context
        using (var context = _repositoryService.AcquireContext())
        {
            if (context == SourceContext.EmptyContext)
            {
                return null;
            }

            var localRepos = _projectManager.AsLocalRepositories(new[]
            {
                _projectProvider.GetDefaultProject()
            });

            var repos = context.Repositories ?? context.PackageSources?.Select(src => _repositoryService.GetRepository(src)) ?? Enumerable.Empty<SourceRepository>();

            return typeFactory.CreateInstanceWithParametersAndAutoCompletion<PackageMetadataProvider>(repos, localRepos);
        }
    }

    /// <summary>
    /// Get installed local metadata based on package.config
    /// </summary>
    /// <param name="token"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<DeferToken> GetMetadataFromLocalSourcesAsync(DeferToken token, CancellationToken cancellationToken)
    {
        var project = _projectProvider.GetDefaultProject();
        var packageId = token.Package.Identity.Id;

        if (project is null)
        {
            return token;
        }

        var installedVersion = await _projectManager.GetVersionInstalledAsync(project, packageId, cancellationToken);

        if (installedVersion is null)
        {
            return token;
        }

        if (_packageMetadataProvider is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("Initialization must be called first");
        }

        var metadata = await _packageMetadataProvider.GetLocalPackageMetadataAsync(new PackageIdentity(packageId, installedVersion), true, cancellationToken);

        token.Result = metadata;

        return token;
    }

    private async Task<DeferToken> GetMetadataFromRemoteSourcesAsync(DeferToken token, CancellationToken cancellationToken)
    {
        if (_settingsProvider is null || _settingsProvider.Model is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("Settings must be initialized first");
        }

        var prerelease = _settingsProvider.Model.IsPreReleaseIncluded;
        if (_packageMetadataProvider is null)
        {
            throw Log.ErrorAndCreateException<InvalidOperationException>("Initialization must be called first");
        }

        var searchMetadata = await _packageMetadataProvider.GetPackageMetadataAsync(token.Package.Identity, prerelease, cancellationToken);

        token.Result = searchMetadata;

        return token;
    }

    public void Add(DeferToken token)
    {
        _taskTokenList.Add(token);
    }
}