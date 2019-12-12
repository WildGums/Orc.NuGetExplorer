namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Enums;
    using NuGetExplorer.Management;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;
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

        private IPackageMetadataProvider _packageMetadataProvider;

        public DefferedPackageLoaderService(IRepositoryContextService repositoryService,
            INuGetPackageManager nuGetExtensibleProjectManager, IExtensibleProjectLocator extensibleProjectLocator, IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);
            Argument.IsNotNull(() => settingsProvider);

            _repositoryService = repositoryService;
            _projectManager = nuGetExtensibleProjectManager;
            _extensibleProjectLocator = extensibleProjectLocator;
            _settignsProvider = settingsProvider;
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

                    //form tasklist
                    var taskList = processedTask.ToDictionary(x => CreateTaskFromToken(x, _aliveCancellationToken));

                    Log.Info($"Start updating {_taskTokenList.Count} items in background");

                    while (taskList.Any())
                    {
                        var nextCompletedTask = await Task.WhenAny(taskList.Keys);

                        var executedToken = taskList[nextCompletedTask];

                        PackageStatus updateStateValue;
                        IPackageSearchMetadata result = null;

                        try
                        {
                            result = await nextCompletedTask;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex, "Cannot get result for background package loading task");
                        }

                        if (result != null)
                        {
                            updateStateValue = await NuGetPackageCombinator.Combine(executedToken.Package, executedToken.LoadType, nextCompletedTask.Result);
                        }
                        else
                        {
                            updateStateValue = PackageStatus.NotInstalled;
                        }

                        taskList.Remove(nextCompletedTask);

                        executedToken.UpdateAction(updateStateValue);
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

        private Task<IPackageSearchMetadata> CreateTaskFromToken(DeferToken token, CancellationToken ct)
        {
            bool prerelease = _settignsProvider.Model.IsPreReleaseIncluded;

            if (token.LoadType == MetadataOrigin.Installed)
            {
                //from local
                return GetMetadataFromLocalSources(token, ct);
            }

            return _packageMetadataProvider.GetPackageMetadataAsync(token.Package.Identity, prerelease, ct);
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

                return new PackageMetadataProvider(repos, localRepos);
            }
        }

        private async Task<IPackageSearchMetadata> GetMetadataFromLocalSources(DeferToken token, CancellationToken ct)
        {
            var metadata = await _packageMetadataProvider.GetLowestLocalPackageMetadataAsync(token.Package.Identity.Id, token.Package.Identity.Version.IsPrerelease, ct);

            return await ValidateIsLocalPackageInstalledAsync(metadata) ? metadata : null;
        }

        /// <summary>
        /// Validate is local package resources has record in packages.config
        /// </summary>
        /// <returns></returns>
        private async Task<bool> ValidateIsLocalPackageInstalledAsync(IPackageSearchMetadata package)
        {
            if (package == null)
            {
                return false;
            }

            var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

            foreach (var project in projects)
            {
                if (await _projectManager.IsPackageInstalledAsync(project, package.Identity, default))
                {
                    return true;
                }
            }

            return false;
        }

        public void Add(DeferToken token)
        {
            _taskTokenList.Add(token);
        }
    }
}
