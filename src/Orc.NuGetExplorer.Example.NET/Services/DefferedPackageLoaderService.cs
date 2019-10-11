using Orc.NuGetExplorer.Enums;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Packaging;
using Orc.NuGetExplorer.Pagination;
using Orc.NuGetExplorer.Providers;

namespace Orc.NuGetExplorer.Services
{
    using Catel;
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Enums;
    using NuGetExplorer.Management;
    using NuGetExplorer.Packaging;
    using NuGetExplorer.Pagination;
    using NuGetExplorer.Providers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class DefferedPackageLoaderService : IDefferedPackageLoaderService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        IList<DeferToken> taskTokenList = new List<DeferToken>();

        private CancellationToken aliveCancellationToken;

        private bool isLoading = false;

        private readonly IPackagesLoaderService _packagesLoaderService;

        private readonly IRepositoryService _repositoryService;

        private readonly INuGetExtensibleProjectManager _projectManager;

        private readonly IExtensibleProjectLocator _extensibleProjectLocator;

        private IPackageMetadataProvider packageMetadataProvider;

        public DefferedPackageLoaderService(IPackagesLoaderService packagesLoaderService, IRepositoryService repositoryService,
            INuGetExtensibleProjectManager nuGetExtensibleProjectManager, IExtensibleProjectLocator extensibleProjectLocator)
        {
            Argument.IsNotNull(() => packagesLoaderService);
            Argument.IsNotNull(() => repositoryService);
            Argument.IsNotNull(() => nuGetExtensibleProjectManager);

            _packagesLoaderService = packagesLoaderService;
            _repositoryService = repositoryService;
            _projectManager = nuGetExtensibleProjectManager;
            _extensibleProjectLocator = extensibleProjectLocator;
        }

        public async Task StartLoadingAsync()
        {
            if (isLoading)
            {
                return;
            }

            await Task.Run(() => RunTaskExecutionLoop());
        }

        private async Task RunTaskExecutionLoop()
        {
            try
            {
                packageMetadataProvider = InitializeMetadataProvider();

                using (var cts = new CancellationTokenSource())
                {
                    aliveCancellationToken = cts.Token;

                    //form tasklist
                    var taskList = taskTokenList.ToDictionary(x => CreateTaskFromToken(x, aliveCancellationToken));

                    Log.Info($"Start updating {taskTokenList.Count} items in background");

                    while (taskList.Count > 0)
                    {
                        var nextCompletedTask = await Task.WhenAny(taskList.Keys);

                        var executedToken = taskList[nextCompletedTask];

                        PackageStatus updateStateValue;

                        if (nextCompletedTask.Result != null)
                        {
                            updateStateValue = await NuGetPackageCombinator.Combine(executedToken.Package, executedToken.LoadType, nextCompletedTask.Result);
                        }
                        else
                        {
                            updateStateValue = PackageStatus.NotInstalled;
                        }

                        taskList.Remove(nextCompletedTask);

                        await Task.Run(() => executedToken.UpdateAction(updateStateValue));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Background loading task was failed");
            }
            finally
            {
                //todo try to complete all remaining tasks
                taskTokenList.Clear();
                isLoading = false;
            }
        }

        private Task<IPackageSearchMetadata> CreateTaskFromToken(DeferToken token, CancellationToken ct)
        {
            if (token.LoadType == Enums.MetadataOrigin.Installed)
            {
                //from local
                return packageMetadataProvider.GetLowestLocalPackageMetadataAsync(token.Package.Identity.Id, token.Package.Identity.Version.IsPrerelease, ct);
            }

            return Task.Run(() => packageMetadataProvider.GetPackageMetadataAsync(token.Package.Identity, token.Package.Identity.Version.IsPrerelease, ct));
        }

        public IPackageMetadataProvider InitializeMetadataProvider()
        {

            //todo provide more automatic way
            //create package metadata provider from context
            using (var context = _repositoryService.AcquireContext())
            {
                var projects = _extensibleProjectLocator.GetAllExtensibleProjects();

                var localRepos = _projectManager.AsLocalRepositories(projects);

                var repos = context.Repositories ?? context.PackageSources.Select(src => _repositoryService.GetRepository(src));

                return new PackageMetadataProvider(repos, localRepos);
            }
        }

        public void Add(DeferToken token)
        {
            taskTokenList.Add(token);
        }
    }
}
