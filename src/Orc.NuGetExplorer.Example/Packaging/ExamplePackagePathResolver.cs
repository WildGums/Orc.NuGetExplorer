namespace Orc.NuGetExplorer.Example.Packaging;

using System;
using System.IO;
using Catel.Services;
using NuGet.Packaging;
using NuGet.Packaging.Core;

/// <summary>
/// The example of custom implementation of default PackagePathResolver supporting multiple roots
/// </summary>
public class ExamplePackagePathResolver : PackagePathResolver
{
    private readonly IAppDataService _appDataService;
    private readonly string _appRootDirectory;

    public ExamplePackagePathResolver(IAppDataService appDataService)
        : this(Path.Combine(appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming), "libs"), true)
    {
        ArgumentNullException.ThrowIfNull(appDataService);

        _appDataService = appDataService;
        _appRootDirectory = Path.Combine(_appDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming), "apps");
    }

    public ExamplePackagePathResolver(string rootDirectory, bool useSideBySidePaths = true)
        : base(rootDirectory, useSideBySidePaths)
    {
    }

    public string AppRootDirectory => _appRootDirectory;

    public override string GetInstallPath(PackageIdentity packageIdentity)
    {
        ArgumentNullException.ThrowIfNull(packageIdentity);

        if (packageIdentity.Id.Equals(PackageScope.Id))
        {
            return Path.Combine(_appRootDirectory, GetPackageDirectoryName(packageIdentity));
        }

        return base.GetInstallPath(packageIdentity);
    }

    public PackageIdentity PackageScope { get; set; } = new PackageIdentity("Orc.NuGetExplorer", new NuGet.Versioning.NuGetVersion(1, 0, 0));
}
