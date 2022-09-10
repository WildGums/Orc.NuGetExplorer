namespace Orc.NuGetExplorer.Tests
{
    using NuGet.Protocol.Core.Types;

    public abstract class TestFixtureBase
    {
        public static IPackageSearchMetadata CreatePackageSearchMetadata(string id, string version)
        {
            var builder = PackageSearchMetadataBuilder.FromIdentity(new NuGet.Packaging.Core.PackageIdentity(id, new NuGet.Versioning.NuGetVersion(version)));
            var package = builder.Build();

            return package;
        }
    }
}
