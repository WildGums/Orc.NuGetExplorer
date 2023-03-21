namespace Orc.NuGetExplorer.Tests.Packaging;

using Catel.IoC;
using Catel.Services;
using NuGet.Packaging;
using NUnit.Framework;

[TestFixture]
public class PackagePathResolverFacts
{
    /// <summary>
    /// This test serves only for informational purposes
    /// </summary>
    [Test]
    [Explicit]
    public void CheckPathAreExpected()
    {
        var applicationDataService = ServiceLocator.Default.ResolveType<IAppDataService>();
        var contentPath = System.IO.Path.Combine(applicationDataService.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming),
            @"WildGums\PM\");
        var pathResolver = new PackagePathResolver(contentPath);
        var pathResolverNoSideBySide = new PackagePathResolver(contentPath, false);

        var dummyPackage = new NuGet.Packaging.Core.PackageIdentity("MyApp", new NuGet.Versioning.NuGetVersion(1, 0, 0));
        var installPath = pathResolver.GetInstallPath(dummyPackage);
        var installedPackageFilePath = pathResolver.GetInstalledPackageFilePath(dummyPackage);
        var installedPath = pathResolver.GetInstalledPath(dummyPackage);
        var packageDirectoryName = pathResolver.GetPackageDirectoryName(dummyPackage);
        var packageFileName = pathResolver.GetPackageFileName(dummyPackage);

        // We are expecting no packages installed on this path, therefore all "installed" path methods returning null
        Assert.IsNull(installedPackageFilePath);
        Assert.IsNull(installedPath);

        Assert.AreEqual("C:\\Users\\JustAnotherUser\\AppData\\Roaming\\Microsoft Corporation\\Microsoft.TestHost\\WildGums\\PM\\MyApp.1.0.0", installPath);
        Assert.AreEqual("MyApp.1.0.0.nupkg", packageFileName);
        Assert.AreEqual("MyApp.1.0.0", packageDirectoryName);
    }
}
