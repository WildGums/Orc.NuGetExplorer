namespace Orc.NuGetExplorer.Tests.Packaging
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Packaging;

    public class MultiVersionPackageSearchMetadataFacts
    {
        private static IPackageSearchMetadata CreatePackageMetadata(string id, string version)
        {
            var builder = PackageSearchMetadataBuilder.FromIdentity(new NuGet.Packaging.Core.PackageIdentity(id, new NuGet.Versioning.NuGetVersion(version)));
            var package = builder.Build();

            return package;
        }

        [TestFixture]
        public class TheToStringMethod
        {
            [TestCase]
            public void Returns_Correct_Information_Without_Identity()
            {
                var availableVersions = new List<IPackageSearchMetadata>
                {
                    CreatePackageMetadata("MyPackage", "1.0.0-alpha0023"),
                    CreatePackageMetadata("MyPackage", "1.0.0-beta0002"),
                    CreatePackageMetadata("MyPackage", "1.0.0")
                };

                var multiVersionPackageSearchMetadata = new MultiVersionPackageSearchMetadata(availableVersions);

                var stringVersion = multiVersionPackageSearchMetadata.ToString();

                Assert.IsFalse(stringVersion.Contains(nameof(MultiVersionPackageSearchMetadata)));
                Assert.AreEqual("(no identity)", stringVersion); 
            }

            [TestCase]
            public void Returns_Correct_Information_With_Identity()
            {
                var availableVersions = new List<IPackageSearchMetadata>
                {
                    CreatePackageMetadata("MyPackage", "1.0.0-alpha0023"),
                    CreatePackageMetadata("MyPackage", "1.0.0-beta0002"),
                    CreatePackageMetadata("MyPackage", "1.0.0")
                };

                var multiVersionPackageSearchMetadata = new MultiVersionPackageSearchMetadata(availableVersions);
                multiVersionPackageSearchMetadata.Identity = availableVersions.Last().Identity;

                var stringVersion = multiVersionPackageSearchMetadata.ToString();

                Assert.IsFalse(stringVersion.Contains(nameof(MultiVersionPackageSearchMetadata)));
                Assert.AreEqual("MyPackage (1.0.0)", stringVersion);
            }
        }
    }
}
