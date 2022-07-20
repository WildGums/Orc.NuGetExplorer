namespace Orc.NuGetExplorer.Tests.TestCases
{
    using Moq;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Models;

    internal static class FixtureNuGetPackageFactory
    {
        public static NuGetPackage CreateFixturePackage(string version, string identity, Enums.MetadataOrigin origin = Enums.MetadataOrigin.Updates)
        {
            var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
            packageSearchMetadataMock.Setup(x => x.Identity).Returns(() => new NuGet.Packaging.Core.PackageIdentity(identity, new NuGet.Versioning.NuGetVersion(version)));

            return new NuGetPackage(packageSearchMetadataMock.Object, origin);
        }
    }
}
