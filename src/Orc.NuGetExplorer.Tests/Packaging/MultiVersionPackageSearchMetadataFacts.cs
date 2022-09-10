namespace Orc.NuGetExplorer.Tests.Packaging
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Packaging;

    public class MultiVersionPackageSearchMetadataFacts
    {
        [TestFixture]
        public class TheToStringMethod : TestFixtureBase
        {
            [TestCase]
            public void Returns_Correct_Information_Without_Identity()
            {
                var availableVersions = new List<IPackageSearchMetadata>
                {
                    CreatePackageSearchMetadata("MyPackage", "1.0.0-alpha0023"),
                    CreatePackageSearchMetadata("MyPackage", "1.0.0-beta0002"),
                    CreatePackageSearchMetadata("MyPackage", "1.0.0")
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
                    CreatePackageSearchMetadata("MyPackage", "1.0.0-alpha0023"),
                    CreatePackageSearchMetadata("MyPackage", "1.0.0-beta0002"),
                    CreatePackageSearchMetadata("MyPackage", "1.0.0")
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
