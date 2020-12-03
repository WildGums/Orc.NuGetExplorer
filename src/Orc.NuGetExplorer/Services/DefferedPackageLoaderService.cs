namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Enums;
    using NuGetExplorer.Management;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;
    using Orc.FileSystem;
    using Orc.NuGetExplorer.Models;

    internal class DefferedPackageLoaderService : IDefferedPackageLoaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IList<DeferToken> _taskTokenList = new List<DeferToken>();

        private CancellationToken _aliveCancellationToken;

        private bool _isLoading = false;

        private readonly IRepositoryContextService _repositoryService;
        private readonly INuGetPackageManager _projectManager;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly IModelProvider<ExplorerSettingsContainer> _settignsProvider;
        private readonly IDirectoryService _directoryService;

        private IPackageMetadataProvider _packageMetadataProvider;

        public DefferedPackageLoaderService(IRepositoryContextService repositoryService, INuGetPackageManager nuGetExtensibleProjectManager, 
            IExtensibleProjectLocator extensibleProjectLocator, IModelProvider<ExplorerSettingsContainer> settingsProvider,
            IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);
            Argument.IsNotNull(() => settingsProvider);
            Argument.IsNotNull(() => directoryService);

            _repositoryService = repositoryService;
            _projectManager = nuGetExtensibleProjectManager;
            _extensibleProjectLocator = extensibleProjectLocator;
            _settignsProvider = settingsProvider;
            _directoryService = directoryService;
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

                if (_packageMetadataProvider == null)
                {
                    Log.Info("Cannot acquire metadata provider for background loading tasks");
                    return;
                }

                using (var cts = new CancellationTokenSource())
                {
                    _aliveCancellationToken = cts.Token;

                    var taskList = processedTask.ToDictionary(x => CreateTaskFromToken(x, _aliveCancellationToken));

                    Log.Info($"Start updating {_taskTokenList.Count} items in background");

                    while (taskList.Any())
                    {
                        var nextCompletedTask = await Task.WhenAny(taskList.Keys);

                        PackageStatus updateStateValue;
                        IPackageSearchMetadata result = null;
                        DeferToken executedToken = null;

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

                        if (result != null)
                        {
                            updateStateValue = await NuGetPackageCombinator.Combine(executedToken.Package, executedToken.LoadType, result);
                        }
                        else
                        {
                            updateStateValue = PackageStatus.NotInstalled;
                        }

                        executedToken?.UpdateAction(updateStateValue);
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

        private Task<DeferToken> CreateTaskFromToken(DeferToken token, CancellationToken cancellationToken)
        {
            bool prerelease = _settignsProvider.Model.IsPreReleaseIncluded;

            if (token.LoadType == MetadataOrigin.Installed)
            {
                //from local
                return GetMetadataFromLocalSources(token, cancellationToken);
            }

            return GetMetadataFromRemoteSources(token, cancellationToken);
        }

        public IPackageMetadataProvider InitializeMetadataProvider()
        {

            //todo provide more automatic way
            //create package metadata provider from context
            using (var context = _repositoryService.AcquireContext())
            {
                if (context == SourceContext.EmptyContext)
                {
                    return null;
                }

                var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

                var localRepos = _projectManager.AsLocalRepositories(projects);

                var repos = context.Repositories ?? context.PackageSources.Select(src => _repositoryService.GetRepository(src));

                return new PackageMetadataProvider(_directoryService, repos, localRepos);
            }
        }

        /// <summary>
        /// Get installed local metadata based on package.config
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DeferToken> GetMetadataFromLocalSources(DeferToken token, CancellationToken cancellationToken)
        {
            var project = _extensibleProjectLocator.GetAllExtensibleProjects().FirstOrDefault();
            string packageId = token.Package.Identity.Id;

            if (project == null)
            {
                return token;
            }

            var installedVersion = await _projectManager.GetVersionInstalledAsync(project, packageId, cancellationToken);

            if (installedVersion == null)
            {
                return token;
            }

            var metadata = await _packageMetadataProvider.GetLocalPackageMetadataAsync(new PackageIdentity(packageId, installedVersion), true, cancellationToken);

            token.Result = metadata;

            return token;
        }

        private async Task<DeferToken> GetMetadataFromRemoteSources(DeferToken token, CancellationToken cancellationToken)
        {
            bool prerelease = _settignsProvider.Model.IsPreReleaseIncluded;

            var searchMetadata = await _packageMetadataProvider.GetPackageMetadataAsync(token.Package.Identity, prerelease, cancellationToken);

            token.Result = searchMetadata;

            return token;
        }

        public void Add(DeferToken token)
        {
            _taskTokenList.Add(token);
        }
    }
}
