﻿namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using Catel.Services;
    using NuGet.Configuration;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.ProjectManagement;
    using NuGet.Protocol;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using Orc.NuGetExplorer.Packaging;
    using Orc.NuGetExplorer.Services;

    internal partial class NuGetProjectPackageManager : INuGetPackageManager, IDisposable
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly SemaphoreSlim UpdateLocker = new SemaphoreSlim(1, 1);

        private readonly IPackageInstallationService _packageInstallationService;
        private readonly INuGetProjectContextProvider _nuGetProjectContextProvider;
        private readonly INuGetProjectConfigurationProvider _nuGetProjectConfigurationProvider;
        private readonly IFileSystemService _fileSystemService;
        private readonly IMessageService _messageService;

        private BatchOperationToken _batchToken;
        private BatchUpdateToken _updateToken;
        private bool _disposedValue;

        public NuGetProjectPackageManager(IPackageInstallationService packageInstallationService,
            INuGetProjectContextProvider nuGetProjectContextProvider, INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider,
            IMessageService messageService, IFileSystemService fileSystemService)
        {
            Argument.IsNotNull(() => packageInstallationService);
            Argument.IsNotNull(() => nuGetProjectContextProvider);
            Argument.IsNotNull(() => nuGetProjectConfigurationProvider);
            Argument.IsNotNull(() => messageService);

            _packageInstallationService = packageInstallationService;
            _nuGetProjectContextProvider = nuGetProjectContextProvider;
            _nuGetProjectConfigurationProvider = nuGetProjectConfigurationProvider;
            _messageService = messageService;
            _fileSystemService = fileSystemService;
        }

        public event AsyncEventHandler<InstallNuGetProjectEventArgs> Install;

        private async Task OnInstallAsync(IExtensibleProject project, PackageIdentity package, bool result)
        {
            var args = new InstallNuGetProjectEventArgs(project, package, result);

            if (_batchToken is not null && !_batchToken.IsDisposed)
            {
                _batchToken.Add(new BatchedInstallNuGetProjectEventArgs(args));
                return;
            }

            if (_updateToken is not null && !_updateToken.IsDisposed)
            {
                _updateToken.Add(args);
                return;
            }

            await Install.SafeInvokeAsync(this, args);
        }

        public event AsyncEventHandler<UninstallNuGetProjectEventArgs> Uninstall;

        private async Task OnUninstallAsync(IExtensibleProject project, PackageIdentity package, bool result)
        {
            var args = new UninstallNuGetProjectEventArgs(project, package, result);

            if (_batchToken is not null && !_batchToken.IsDisposed)
            {
                _batchToken.Add(new BatchedUninstallNuGetProjectEventArgs(args));
                return;
            }

            if (_updateToken is not null && !_updateToken.IsDisposed)
            {
                _updateToken.Add(args);
                return;
            }

            await Uninstall.SafeInvokeAsync(this, args);
        }

        public event AsyncEventHandler<UpdateNuGetProjectEventArgs> Update;

        private async Task OnUpdateAsync(UpdateNuGetProjectEventArgs args)
        {
            if (_batchToken is not null && !_batchToken.IsDisposed)
            {
                _batchToken.Add(args);
                return;
            }

            await Update.SafeInvokeAsync(this, args);
        }

        public async Task<IEnumerable<PackageReference>> GetInstalledPackagesAsync(IExtensibleProject project, CancellationToken token)
        {
            // TODO should local metadata is also be checked?

            var packageConfigProject = _nuGetProjectConfigurationProvider.GetProjectConfig(project);

            var packageReferences = await packageConfigProject.GetInstalledPackagesAsync(token);

            return packageReferences;
        }

        /// <summary>
        /// Creates cross-project collection of installed package references
        /// grouped by id and version
        /// </summary>
        /// <param name="projects"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<PackageCollection> CreatePackagesCollectionFromProjectsAsync(IEnumerable<IExtensibleProject> projects, CancellationToken cancellationToken)
        {
            // Read package references from all projects.
            var tasks = projects
                .Select(project => GetInstalledPackagesAsync(project, cancellationToken));
            var packageReferences = await Task.WhenAll(tasks);

            // Group all package references for an id/version into a single item.
            var packages = packageReferences
                    .SelectMany(e => e)
                    .GroupBy(e => e.PackageIdentity, (key, group) => new PackageCollectionItem(key.Id, key.Version, group))
                    .ToArray();

            return new PackageCollection(packages);
        }

        /// <summary>
        /// Checks is package installed from records in config repository
        /// </summary>
        /// <param name="project"></param>
        /// <param name="package"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<bool> IsPackageInstalledAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token)
        {
            Argument.IsNotNull(() => project);
            Argument.IsNotNull(() => package);

            try
            {
                var installedReferences = await GetInstalledPackagesAsync(project, token);

                var installedPackage = installedReferences.FirstOrDefault(x => x.PackageIdentity.Equals(package, NuGet.Versioning.VersionComparison.VersionRelease));

                return installedPackage is not null;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public async Task<bool> IsPackageInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token)
        {
            Argument.IsNotNull(() => project);

            if (string.IsNullOrEmpty(packageId))
            {
                throw Log.ErrorAndCreateException((string message) => new ArgumentException(message, nameof(packageId)), "Cannot be null or empty string");
            }

            try
            {
                var installedReferences = await GetInstalledPackagesAsync(project, token);

                var isInstalled = installedReferences.Any(x => string.Equals(x.PackageIdentity.Id, packageId, StringComparison.OrdinalIgnoreCase));

                return isInstalled;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return false;
            }
        }

        public async Task<NuGetVersion> GetVersionInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token)
        {
            var installedReferences = await GetInstalledPackagesAsync(project, token);

            var installedVersion = installedReferences.Where(x => string.Equals(x.PackageIdentity.Id, packageId) && x.PackageIdentity.HasVersion)
                .Select(x => x.PackageIdentity.Version).FirstOrDefault();

            return installedVersion;
        }

        public async Task<bool> InstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token, bool showErrors = true)
        {
            try
            {
                bool dependencyInstallResult = true;

                var packageConfigProject = _nuGetProjectConfigurationProvider.GetProjectConfig(project);

                var repositories = SourceContext.CurrentContext.Repositories;

                var installerResults = await Task.Run(async () => await _packageInstallationService.InstallAsync(package, project, repositories, project.IgnoreDependencies, token), token);

                if (!installerResults.Result.Any())
                {
                    Log.Error($"Failed to install package {package}");

                    // todo PackageCommandService or context is better place for messaging

                    if (showErrors)
                    {
                        await _messageService.ShowErrorAsync($"Failed to install package {package}.\n{installerResults.ErrorMessage}");
                    }

                    return false;
                }

                await Task.Run(async () =>
                {
                    foreach (var packageDownloadResultPair in installerResults.Result)
                    {
                        var dependencyIdentity = packageDownloadResultPair.Key;
                        var downloadResult = packageDownloadResultPair.Value;

                        try
                        {
                            var result = await packageConfigProject.InstallPackageAsync(
                                dependencyIdentity,
                                downloadResult,
                                _nuGetProjectContextProvider.GetProjectContext(FileConflictAction.PromptUser),
                                token);

                            dependencyInstallResult &= result;
                        }
                        catch (InvalidOperationException ex)
                        {
                            Log.Error($"Saving package configuration failed in project {project} when installing package {package}");
                            Log.Error(ex);
                            dependencyInstallResult &= false;
                        }
                    }
                }, token);

                await OnInstallAsync(project, package, dependencyInstallResult || project.IgnoreDependencies);

                return true;
            }
            catch (ProjectInstallException ex)
            {
                Log.Error($"Failed to install package {package}");

                if (showErrors)
                {
                    await _messageService.ShowErrorAsync($"Failed to install package {package}.\n{ex.Message}");
                }

                if (ex?.CurrentBatch is null)
                {
                    return false;
                }

                // Mark all partially-extracted packages to removal;
                foreach (var canceledPackage in ex.CurrentBatch)
                {
                    _fileSystemService.CreateDeleteme(canceledPackage.Id, project.GetInstallPath(canceledPackage));
                }

                // throw exception to add exception in operation context
                throw;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"The Installation of package {package} was failed");
                throw;
            }
        }

        public async Task InstallPackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token)
        {
            using (_batchToken = new BatchOperationToken())
            {
                foreach (var project in projects)
                {
                    await InstallPackageForProjectAsync(project, package, token);
                }
            }

            // raise supressed events
            foreach (var args in _batchToken.GetInvokationList<InstallNuGetProjectEventArgs>())
            {
                await Install.SafeInvokeAsync(this, args);
            }
        }

        public async Task UninstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package, CancellationToken token)
        {
            try
            {
                var installedPackages = await GetInstalledPackagesAsync(project, token);

                await _packageInstallationService.UninstallAsync(package, project, installedPackages, token);

                await OnUninstallAsync(project, package, true);
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Uninstall of package {package} was failed");
            }
        }

        public async Task UninstallPackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package, CancellationToken token)
        {
            using (_batchToken = new BatchOperationToken())
            {
                foreach (var project in projects)
                {
                    await UninstallPackageForProjectAsync(project, package, token);
                }
            }

            // raise supressed events
            foreach (var args in _batchToken.GetInvokationList<UninstallNuGetProjectEventArgs>())
            {
                await Uninstall.SafeInvokeAsync(this, args);
            }
        }

        public async Task UpdatePackageForProjectAsync(IExtensibleProject project, string packageid, NuGetVersion targetVersion, CancellationToken token)
        {
            try
            {
                var version = await GetVersionInstalledAsync(project, packageid, token);

                using (_updateToken = new BatchUpdateToken(new PackageIdentity(packageid, version)))
                {
                    await UpdatePackageAsync(project, new PackageIdentity(packageid, version), targetVersion, token);
                }

                var updates = _updateToken.GetUpdateEventArgs();

                foreach (var updateArg in updates)
                {
                    await OnUpdateAsync(updateArg);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error during package {packageid} update");
                throw;
            }
        }

        public async Task UpdatePackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, string packageid, NuGetVersion targetVersion, CancellationToken token)
        {
            try
            {
                using (_updateToken = new BatchUpdateToken(new PackageIdentity(packageid, targetVersion)))
                {
                    foreach (var project in projects)
                    {
                        var version = await GetVersionInstalledAsync(project, packageid, token);

                        await UpdatePackageAsync(project, new PackageIdentity(packageid, version), targetVersion, token);
                    }
                }

                var updates = _updateToken.GetUpdateEventArgs();

                foreach (var updateArg in updates)
                {
                    await OnUpdateAsync(updateArg);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Error during package {packageid} update");
            }
        }

        private async Task UpdatePackageAsync(IExtensibleProject project, PackageIdentity installedVersion, NuGetVersion targetVersion, CancellationToken token)
        {
            try
            {
                await UpdateLocker.WaitAsync(token);

                await UninstallPackageForProjectAsync(project, installedVersion, token);
                await InstallPackageForProjectAsync(project, new PackageIdentity(installedVersion.Id, targetVersion), token);
            }
            finally
            {
                UpdateLocker.Release();
            }
        }

        public IEnumerable<SourceRepository> AsLocalRepositories(IEnumerable<IExtensibleProject> projects)
        {
            var repos = projects.Select(x =>
                 new SourceRepository(
                        new PackageSource(x.ContentPath), Repository.Provider.GetCoreV3(), FeedType.FileSystemV2
                ));


            return repos;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _batchToken?.Dispose();
                    _updateToken?.Dispose();
                }

                _disposedValue = true;
            }
        }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
