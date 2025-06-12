namespace Orc.NuGetExplorer.Tests.Services
{
    using System.Threading.Tasks;
    using Catel.IoC;
    using Catel.Services;
    using Moq;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Packaging;
    using VerifyNUnit;

    public class ApiPackageRegistryFacts
    {
        [TestFixture]
        public class The_IsRegistered_Method
        {
            [Test]
            public async Task Returns_True_For_Registered_Package_Async()
            {
                var apiPackageRegistry = new ApiPackageRegistry(new LanguageService());

                apiPackageRegistry.Register("MyApp.Api", "2.0.0-alpha.9999");

                Assert.That(apiPackageRegistry.IsRegistered("MyApp.Api"), Is.True);
            }

            [Test]
            public async Task Returns_False_For_Unregistered_Package_Async()
            {
                var apiPackageRegistry = new ApiPackageRegistry(new LanguageService());

                apiPackageRegistry.Register("MyApp.Api", "2.0.0-alpha.9999");

                Assert.That(apiPackageRegistry.IsRegistered("OtherApp.Api"), Is.False);
            }
        }

        [TestFixture]
        public class The_Validate_Method
        {
            [Test]
            public async Task Returns_Error_When_Current_Version_Is_Smaller_Than_Minimum_Required_Version_Async()
            {
                var languageService = ServiceLocator.Default.ResolveRequiredType<ILanguageService>();

                var apiPackageRegistry = new ApiPackageRegistry(languageService);

                apiPackageRegistry.Register("MyApp.Api", "2.0.0-alpha.9999");

                var packageIdentity = new PackageIdentity("MyPackage", new NuGet.Versioning.NuGetVersion("1.0.0"));

                var dependencyGroup = new PackageDependencyGroup(NuGetFramework.AnyFramework,
                    new[]
                    {
                        new PackageDependency("MyApp.Api", new VersionRange(new NuGet.Versioning.NuGetVersion("3.0.0-alpha.9999")))
                    });

                var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
                packageSearchMetadataMock.SetupGet(x => x.Identity)
                    .Returns(packageIdentity);
                packageSearchMetadataMock.Setup(x => x.DependencySets)
                    .Returns(new[]
                    {
                        dependencyGroup
                    });

                var packageDetails = new PackageDetails(packageSearchMetadataMock.Object, packageIdentity, false);

                apiPackageRegistry.Validate(packageDetails);

                await Verifier.Verify(packageDetails.ValidationContext.GetValidations());
            }
        }
    }
}
