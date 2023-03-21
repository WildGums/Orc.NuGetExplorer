namespace Orc.NuGetExplorer.Tests;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;

public static class GlobalMocks
{
    public static NuGetPackage CreateMockPackage(string version, string identity, Enums.MetadataOrigin origin = Enums.MetadataOrigin.Updates)
    {
        var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
        packageSearchMetadataMock.Setup(x => x.Identity).Returns(() => new NuGet.Packaging.Core.PackageIdentity(identity, new NuGet.Versioning.NuGetVersion(version)));

        return new NuGetPackage(packageSearchMetadataMock.Object, origin);
    }

    public static IRepository CreateMockRepository(string name, string path)
    {
        var projectRepositoryMock = new Mock<IRepository>();
        projectRepositoryMock.Setup(x => x.Source).Returns(() => (Path.Combine(Environment.CurrentDirectory, path)));
        projectRepositoryMock.Setup(x => x.Name).Returns(() => name);

        return projectRepositoryMock.Object;
    }

    public static IExtensibleProject CreateMockProject()
    {
        return new MockProject();
    }

    private class MockProject : IExtensibleProject
    {
        private static readonly string ContentFolder = Path.Combine(Environment.CurrentDirectory, "packages");
        private readonly PackagePathResolver _packageResolver = new PackagePathResolver(ContentFolder);

        public string Name => "Test project";
        public string Framework => "net6.0-windows";
        public string ContentPath => ContentFolder;
        public ImmutableList<NuGetFramework> SupportedPlatforms { get; set; }
        public bool IgnoreDependencies { get; }
        public bool SupportSideBySide => true;
        public bool NoCache { get; }

        public string GetInstallPath(PackageIdentity packageIdentity) => _packageResolver.GetInstallPath(packageIdentity);
        public PackagePathResolver GetPathResolver() => _packageResolver;
    }
}
