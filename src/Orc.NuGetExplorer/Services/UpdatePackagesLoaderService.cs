namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Catel.IoC;
using Catel.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;
using NuGetExplorer.Packaging;
using NuGetExplorer.Providers;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Pagination;
using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

internal class UpdatePackagesLoaderService : IPackageLoaderService, IPackagesUpdatesSearcherService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(UpdatePackagesLoaderService));

    private readonly IRepositoryService _repositoryService;
    private readonly IExtensibleProjectLocator _extensibleProjectLocator;
    private readonly INuGetPackageManager _nuGetExtensibleProjectManager;
    private readonly IPackageValidatorProvider _packageValidatorProvider;

    //underlying service
    private readonly Lazy<IPackageLoaderService> _feedRepositoryLoader;
    private readonly Lazy<IPackageLoaderService> _projectRepositoryLoader;

    private readonly HashSet<string> _discardedPackagesSet = new();

    public UpdatePackagesLoaderService(IRepositoryService repositoryService, IExtensibleProjectLocator extensibleProjectLocator,
        INuGetPackageManager nuGetExtensibleProjectManager, IPackageValidatorProvider packageValidatorProvider,
        IServiceProvider serviceProvider)
    {
        _repositoryService = repositoryService;
        _extensibleProjectLocator = extensibleProjectLocator;
        _nuGetExtensibleProjectManager = nuGetExtensibleProjectManager;
        _packageValidatorProvider = packageValidatorProvider;

        _feedRepositoryLoader = new Lazy<IPackageLoaderService>(() => serviceProvider.GetRequiredService<IPackageLoaderService>());
        _projectRepositoryLoader = new Lazy<IPackageLoaderService>(() => serviceProvider.GetRequiredKeyedService<IPackageLoaderService>("Installed"));
    }

    public IPackageMetadataProvider? PackageMetadataProvider => _projectRepositoryLoader.Value.PackageMetadataProvider;

    public async Task<IReadOnlyList<IPackageSearchMetadata>> LoadAsync(string searchTerm, PageContinuation pageContinuation, SearchFilter searchFilter, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(pageContinuation);

        try
        {
            if (pageContinuation.Current <= 0)
            {
                //start search from begin, don't skip packages
                _discardedPackagesSet.Clear();
            }

            var localContinuation = new PageContinuation(pageContinuation, !pageContinuation.Source.PackageSources.Any());

            var installedPackagesMetadatas = (await _projectRepositoryLoader.Value.LoadAsync(searchTerm, localContinuation, searchFilter, token));

            pageContinuation.GetNext();

            Logger.LogInformation("Local packages queried for further available update searching");

            var updateList = new List<IPackageSearchMetadata>();

            var validators = _packageValidatorProvider.GetValidators();

            //getting last metadata
            foreach (var package in installedPackagesMetadatas)
            {
                if (_discardedPackagesSet.Contains(package.Identity.Id))
                {
                    continue;
                }

                var clonedMetadata = PackageMetadataProvider is null ? null : await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, searchFilter.IncludePrerelease,
                    (p) =>
                    {
                        foreach (var validator in validators)
                        {
                            var validationContext = validator.Validate(p);
                            if (validationContext.HasErrors)
                            {
                                return false;
                            }
                        }

                        return true;
                    }, token);
                if (clonedMetadata is null)
                {
                    Logger.LogWarning($"Couldn't retrieve update metadata for installed {package.Identity}");
                    continue;
                }

                if (clonedMetadata.Identity.Version > package.Identity.Version)
                {
                    var combinedMetadata = UpdatePackageSearchMetadataBuilder.FromMetadatas((ClonedPackageSearchMetadata)clonedMetadata, package).Build();
                    updateList.Add(combinedMetadata);
                }

                _discardedPackagesSet.Add(package.Identity.Id);

                if (updateList.Count >= pageContinuation.Size)
                {
                    break;
                }
            }

            return updateList;
        }
        catch (Exception ex) when (token.IsCancellationRequested)
        {
            throw Logger.LogErrorAndCreateException<OperationCanceledException>("Search request was canceled", ex, token);
        }
    }

    #region IPackagesUpdatesSearcherService

    public async Task<IReadOnlyList<IPackageSearchMetadata>> SearchForPackagesUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
    {
        var updateList = new List<IPackageSearchMetadata>();

        try
        {
            Logger.LogDebug("Searching for updates, allowPrerelease = {0}, authenticateIfRequired = {1}", allowPrerelease, authenticateIfRequired);

            // Get local packages
            var localRepo = _repositoryService.LocalRepository;

            var installedPackagesMetadatas = await _projectRepositoryLoader.Value.LoadWithDefaultsAsync(localRepo.Source, token);

            var validators = _packageValidatorProvider.GetValidators();

            // Getting updates
            foreach (var package in installedPackagesMetadatas)
            {
                // Current behavior defined based on installed version
                // Pre-release versions upgraded to latest stable or pre-release
                // Stable versions upgraded to latest stable only
                var isPrereleaseUpdate = allowPrerelease ?? package.Identity.Version.IsPrerelease;

                var clonedMetadata = PackageMetadataProvider is null ? null : await PackageMetadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, isPrereleaseUpdate,
                    (p) =>
                    {
                        foreach (var validator in validators)
                        {
                            var validationContext = validator.Validate(p);
                            if (validationContext.HasErrors)
                            {
                                return false;
                            }
                        }

                        return true;
                    }, token);
                if (clonedMetadata is null)
                {
                    Logger.LogWarning($"Couldn't retrieve update metadata for installed {package.Identity}");
                    continue;
                }

                // TODO: Check target framework

                if (clonedMetadata.Identity.Version > package.Identity.Version)
                {
                    //construct package details
                    updateList.Add(clonedMetadata);
                }
            }

            return updateList;
        }
        catch (Exception ex) when (token.IsCancellationRequested)
        {
            throw Logger.LogErrorAndCreateException<OperationCanceledException>("Search request was canceled", ex, token);
        }
    }

    public async Task<IReadOnlyList<IPackageDetails>> SearchForUpdatesAsync(bool? allowPrerelease = null, bool authenticateIfRequired = true, CancellationToken token = default)
    {
        return (await SearchForPackagesUpdatesAsync(allowPrerelease, authenticateIfRequired, token))
            .Select(m => PackageDetailsFactory.Create(PackageOperationType.Update, m, m.Identity, true))
            .ToArray();
    }

    public async Task<IReadOnlyList<IPackageDetails>> SearchForUpdatesAsync(string[] excludeReleaseTags, bool? allowPrerelease = null, CancellationToken token = default)
    {
        var foundUpdates = (await SearchForPackagesUpdatesAsync(allowPrerelease, true, token)).ToList();

        // Replace all packages with restricted tag with nearest possible
        var packagesToExclude = foundUpdates.Where(p => p.Identity.Version.Release.ContainsAny(excludeReleaseTags, StringComparison.OrdinalIgnoreCase)).ToList();
        var metadataProvider = PackageMetadataProvider;

        if (packagesToExclude.Any())
        {
            var localRepositorySource = _repositoryService.LocalRepository.Source;
            var installedPackagesMetadatas = await _projectRepositoryLoader.Value.LoadWithDefaultsAsync(localRepositorySource, token);

            var validators = _packageValidatorProvider.GetValidators();

            foreach (var package in packagesToExclude)
            {
                foundUpdates.Remove(package);

                var metadata = metadataProvider is null ? null : await metadataProvider.GetHighestPackageMetadataAsync(package.Identity.Id, allowPrerelease ?? true, excludeReleaseTags, 
                    (p) =>
                    {
                        foreach (var validator in validators)
                        {
                            var validationContext = validator.Validate(p);
                            if (validationContext.HasErrors)
                            {
                                return false;
                            }
                        }

                        return true;
                    }, token);
                if (metadata is null)
                {
                    Logger.LogDebug($"Couldn't retrieve update metadata for installed package {package.Identity.Id}");
                    continue;
                }

                var localPackage = installedPackagesMetadatas.FirstOrDefault(p => string.Equals(p.Identity.Id, metadata.Identity.Id));
                if (localPackage is null)
                {
                    // Normally shouldn't happen
                    Logger.LogDebug($"Couldn't match retrieved update metadata with any local package");
                    continue;
                }

                if (metadata.Identity.Version > localPackage.Identity.Version)
                {
                    // Add as replacement for alpha
                    foundUpdates.Add(metadata);
                }
            }
        }

        var updatePackages = foundUpdates.Select(p => PackageDetailsFactory.Create(PackageOperationType.Update, p, p.Identity, true)).ToList();
        return updatePackages;
    }
    #endregion
}
