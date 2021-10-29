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
    using Orc.NuGetExplorer.Models;

    internal class DefferedPackageLoaderService : IDefferedPackageLoaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IList<DeferToken> _taskTokenList = new List<DeferToken>();

        private CancellationToken _aliveCancellationToken;

        private bool _isLoading = false;

        private readonly INuGetPackageManager _projectManager;
        private readonly IExtensibleProjectLocator _extensibleProjectLocator;
        private readonly IModelProvider<ExplorerSettingsContainer> _settignsProvider;
        private IPackageMetadataProvider _packageMetadataProvider;

        public DefferedPackageLoaderService(INuGetPackageManager nuGetExtensibleProjectManager, IExtensibleProjectLocator extensibleProjectLocator, IModelProvider<ExplorerSettingsContainer> settingsProvider)
        {
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);
            Argument.IsNotNull(() => settingsProvider);

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

                using (var cts = new CancellationTokenSource())
                using (var sourceContext = SourceContext.AcquireContext())
                {
                    _aliveCancellationToken = cts.Token;
                    _packageMetadataProvider = sourceContext.PackageMetadataProviderValue;

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

                        if (result is null || executedToken is null)
                        {
                            updateStateValue = PackageStatus.NotInstalled;
                        }
                        else
                        {
                            updateStateValue = await NuGetPackageCombinator.CombineAsync(executedToken.Package, executedToken.LoadType, result);
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
#pragma warning disable CL0002 // Use async suffix
        private Task<DeferToken> CreateTaskFromToken(DeferToken token, CancellationToken cancellationToken)
        {
            bool prerelease = _settignsProvider.Model.IsPreReleaseIncluded;

            if (token.LoadType == MetadataOrigin.Installed)
            {
                //from local
                return GetMetadataFromLocalSourcesAsync(token, cancellationToken);
            }

            return GetMetadataFromRemoteSourcesAsync(token, cancellationToken);
        }
#pragma warning restore CL0002 // Use async suffix

        /// <summary>
        /// Get installed local metadata based on package.config
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<DeferToken> GetMetadataFromLocalSourcesAsync(DeferToken token, CancellationToken cancellationToken)
        {
            var project = _extensibleProjectLocator.GetAllExtensibleProjects().FirstOrDefault();
            string packageId = token.Package.Identity.Id;

            if (project is null)
            {
                return token;
            }

            var installedVersion = await _projectManager.GetVersionInstalledAsync(project, packageId, cancellationToken);

            if (installedVersion is null)
            {
                return token;
            }

            var metadata = await _packageMetadataProvider.GetLocalPackageMetadataAsync(new PackageIdentity(packageId, installedVersion), true, cancellationToken);

            token.Result = metadata;

            return token;
        }

        private async Task<DeferToken> GetMetadataFromRemoteSourcesAsync(DeferToken token, CancellationToken cancellationToken)
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
