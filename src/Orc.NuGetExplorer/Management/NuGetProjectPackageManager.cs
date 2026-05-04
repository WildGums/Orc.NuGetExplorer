namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using Catel.Services;
using Microsoft.Extensions.Logging;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.ProjectManagement;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using Packaging;
using Services;

internal partial class NuGetProjectPackageManager : INuGetPackageManager, IDisposable
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(NuGetProjectPackageManager));

    private static readonly SemaphoreSlim UpdateLocker = new(1, 1);

    private readonly IPackageInstallationService _packageInstallationService;
    private readonly INuGetProjectContextProvider _nuGetProjectContextProvider;
    private readonly INuGetProjectConfigurationProvider _nuGetProjectConfigurationProvider;
    private readonly IFileSystemService _fileSystemService;
    private readonly ILanguageService _languageService;
    private readonly IMessageService _messageService;

    private BatchOperationToken? _batchToken;
    private BatchUpdateToken? _updateToken;
    private bool _disposedValue;

    public NuGetProjectPackageManager(IPackageInstallationService packageInstallationService,
        INuGetProjectContextProvider nuGetProjectContextProvider, INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider,
        IMessageService messageService, IFileSystemService fileSystemService, ILanguageService languageService)
    {
        _packageInstallationService = packageInstallationService;
        _nuGetProjectContextProvider = nuGetProjectContextProvider;
        _nuGetProjectConfigurationProvider = nuGetProjectConfigurationProvider;
        _messageService = messageService;
        _fileSystemService = fileSystemService;
        _languageService = languageService;
    }

    public event AsyncEventHandler<InstallNuGetProjectEventArgs>? Install;

    private async Task OnInstallAsync(IExtensibleProject project, PackageIdentity package, bool result)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(package);

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

    public event AsyncEventHandler<UninstallNuGetProjectEventArgs>? Uninstall;

    private async Task OnUninstallAsync(IExtensibleProject project, PackageIdentity package, bool result)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(package);

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

    public event AsyncEventHandler<UpdateNuGetProjectEventArgs>? Update;

    private async Task OnUpdateAsync(UpdateNuGetProjectEventArgs args)
    {
        if (_batchToken is not null && !_batchToken.IsDisposed)
        {
            _batchToken.Add(args);
            return;
        }

        await Update.SafeInvokeAsync(this, args);
    }

    public async Task<IReadOnlyList<PackageReference>> GetInstalledPackagesAsync(IExtensibleProject project, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);

        // TODO should local metadata is also be checked?

        var packageConfigProject = _nuGetProjectConfigurationProvider.GetProjectConfig(project);

        var packageReferences = await packageConfigProject.GetInstalledPackagesAsync(token);

        return packageReferences.ToArray();
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
        ArgumentNullException.ThrowIfNull(projects);

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
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            var installedReferences = await GetInstalledPackagesAsync(project, token);

            var installedPackage = installedReferences.FirstOrDefault(x => x.PackageIdentity.Equals(package, VersionComparison.VersionRelease));

            return installedPackage is not null;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, null);
            return false;
        }
    }

    public async Task<bool> IsPackageInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);

        if (string.IsNullOrEmpty(packageId))
        {
            throw Logger.LogErrorAndCreateException(message => new ArgumentException(message, nameof(packageId)), "Cannot be null or empty string");
        }

        try
        {
            var installedReferences = await GetInstalledPackagesAsync(project, token);

            var isInstalled = installedReferences.Any(x => string.Equals(x.PackageIdentity.Id, packageId, StringComparison.OrdinalIgnoreCase));

            return isInstalled;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, null);
            return false;
        }
    }

    public async Task<NuGetVersion?> GetVersionInstalledAsync(IExtensibleProject project, string packageId, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);

        var installedReferences = await GetInstalledPackagesAsync(project, token);

        var installedVersion = installedReferences.Where(x => string.Equals(x.PackageIdentity.Id, packageId) && x.PackageIdentity.HasVersion)
            .Select(x => x.PackageIdentity.Version).FirstOrDefault();

        return installedVersion;
    }

    public virtual async Task<bool> InstallPackageForProjectAsync(PackageInstallationContext context, CancellationToken token)
    {
        var package = context.Package;
        var project = context.Project;
        var packagePredicate = context.PackagePredicate;
        var showErrors = context.ShowErrors;
        var ignoreMissingPackages = context.IgnoreMissingPackages;
        var allowMultipleVersions = context.AllowMultipleVersions;

        try
        {
            var dependencyInstallResult = true;

            var packageConfigProject = _nuGetProjectConfigurationProvider.GetProjectConfig(project);

            var repositories = SourceContext.CurrentContext?.Repositories;
            if (repositories is null || !repositories.Any())
            {
                Logger.LogError("Failed to install package {Package}", package);

                if (showErrors)
                {
                    var errorMessage = string.Format(_languageService.GetRequiredString("NuGetExplorer_NuGetProjectPackageManager_Error_NoPackageSource_Template"), package);
                    await _messageService.ShowErrorAsync(errorMessage);
                }

                return false;
            }
            var installationContext = new InstallationContext
            {
                Project = project,
                Repositories = repositories,
                Package = package,
                PackagePredicate = packagePredicate,
                IgnoreMissingPackages = ignoreMissingPackages,
                AllowMultipleVersions = allowMultipleVersions
            };

            var installerResults = await _packageInstallationService.InstallAsync(installationContext, token);
            if (!installerResults.Result.Any())
            {
                if (showErrors)
                {
                    var error = string.Format(_languageService.GetRequiredString("NuGetExplorer_NuGetProjectPackageManager_Error_FailedInstall_Template"), package, installerResults.ErrorMessage);
                    await _messageService.ShowErrorAsync(error);
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
                        Logger.LogError("Saving package configuration failed in project {Project} when installing package {Package}", project, package);
                        Logger.LogError(ex, null);
                        dependencyInstallResult = false;
                    }
                }
            }, token);

            await OnInstallAsync(project, package, dependencyInstallResult || project.IgnoreMissingDependencies);

            return true;
        }
        catch (ProjectInstallException ex)
        {
            Logger.LogError("Failed to install package {Package}", package);

            if (showErrors)
            {
                var error = string.Format(_languageService.GetRequiredString("NuGetExplorer_NuGetProjectPackageManager_Error_FailedInstall_Template"), package, ex.Message);
                await _messageService.ShowErrorAsync(error);
            }

            if (ex.CurrentBatch is null)
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
            Logger.LogError(ex, "The Installation of package {Package} was failed", package);
            throw;
        }
    }

    public async Task InstallPackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package,
        Func<PackageIdentity, bool>? packagePredicate, bool showErrors, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(projects);
        ArgumentNullException.ThrowIfNull(package);

        using (_batchToken = new BatchOperationToken())
        {
            foreach (var project in projects)
            {
                var context = new PackageInstallationContext
                {
                    Package = package,
                    Project = project,
                    PackagePredicate = packagePredicate,
                    AllowMultipleVersions = false,
                    IgnoreMissingPackages = project.IgnoreMissingDependencies,
                    ShowErrors = showErrors
                };

                await InstallPackageForProjectAsync(context, token);
            }
        }

        // raise suppressed events
        foreach (var args in _batchToken.GetInvokationList<InstallNuGetProjectEventArgs>())
        {
            await Install.SafeInvokeAsync(this, args);
        }
    }

    public async Task UninstallPackageForProjectAsync(IExtensibleProject project, PackageIdentity package,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(package);

        try
        {
            var installedPackages = await GetInstalledPackagesAsync(project, token);

            await _packageInstallationService.UninstallAsync(package, project, installedPackages, packagePredicate, token);

            await OnUninstallAsync(project, package, true);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Uninstall of package {Package} was failed", package);
        }
    }

    public async Task UninstallPackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, PackageIdentity package,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(projects);
        ArgumentNullException.ThrowIfNull(package);

        using (_batchToken = new BatchOperationToken())
        {
            foreach (var project in projects)
            {
                await UninstallPackageForProjectAsync(project, package, packagePredicate, token);
            }
        }

        // raise suppressed events
        foreach (var args in _batchToken.GetInvokationList<UninstallNuGetProjectEventArgs>())
        {
            await Uninstall.SafeInvokeAsync(this, args);
        }
    }

    public async Task UpdatePackageForProjectAsync(IExtensibleProject project, string packageId, NuGetVersion targetVersion,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(targetVersion);

        try
        {
            var version = await GetVersionInstalledAsync(project, packageId, token);

            using (_updateToken = new BatchUpdateToken(new PackageIdentity(packageId, version)))
            {
                await UpdatePackageAsync(project, new PackageIdentity(packageId, version), targetVersion, packagePredicate, token);
            }

            var updates = _updateToken.GetUpdateEventArgs();

            foreach (var updateArg in updates)
            {
                await OnUpdateAsync(updateArg);
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error during package {PackageId} update", packageId);
            throw;
        }
    }

    public async Task UpdatePackageForMultipleProjectAsync(IReadOnlyList<IExtensibleProject> projects, string packageId, NuGetVersion targetVersion,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(projects);
        ArgumentNullException.ThrowIfNull(targetVersion);

        try
        {
            using (_updateToken = new BatchUpdateToken(new PackageIdentity(packageId, targetVersion)))
            {
                foreach (var project in projects)
                {
                    var version = await GetVersionInstalledAsync(project, packageId, token);

                    await UpdatePackageAsync(project, new PackageIdentity(packageId, version), targetVersion, packagePredicate, token);
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
            Logger.LogError(ex, "Error during package {PackageId} update", packageId);
        }
    }

    private async Task UpdatePackageAsync(IExtensibleProject project, PackageIdentity installedVersion, NuGetVersion targetVersion,
        Func<PackageIdentity, bool>? packagePredicate, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(project);
        ArgumentNullException.ThrowIfNull(installedVersion);
        ArgumentNullException.ThrowIfNull(targetVersion);

        try
        {
            await UpdateLocker.WaitAsync(token);

            await UninstallPackageForProjectAsync(project, installedVersion, packagePredicate, token);

            var context = new PackageInstallationContext
            {
                Package = new PackageIdentity(installedVersion.Id, targetVersion),
                Project = project,
                PackagePredicate = packagePredicate,
                AllowMultipleVersions = false,
                IgnoreMissingPackages = project.IgnoreMissingDependencies,
                ShowErrors = true
            };

            await InstallPackageForProjectAsync(context, token);
        }
        finally
        {
            UpdateLocker.Release();
        }
    }

    public IReadOnlyList<SourceRepository> AsLocalRepositories(IEnumerable<IExtensibleProject> projects)
    {
        ArgumentNullException.ThrowIfNull(projects);

        var repos = projects.Select(x =>
            new SourceRepository(
                new PackageSource(x.ContentPath), Repository.Provider.GetCoreV3(), FeedType.FileSystemV2
            ));


        return repos.ToArray();
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
