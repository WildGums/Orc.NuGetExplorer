namespace Orc.NuGetExplorer.Tests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using NuGet.Frameworks;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework;
    using VerifyNUnit;

    public class PackageTargetFrameworkValidatorFacts
    {
        [TestFixture]
        public class The_Validate_Method
        {
            [Test]
            public async Task Returns_Error_When_TargetFramework_Is_Too_High_Async()
            {
                var defaultNuGetFrameworkMock = new Mock<IDefaultNuGetFramework>();
                defaultNuGetFrameworkMock.Setup(x => x.GetHighest())
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new NuGetFramework(".NETFramework", new Version(4, 7, 2)),
                            new NuGetFramework(".NETCoreApp", new Version(8, 0))
                        };
                    });

                var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
                packageSearchMetadataMock.Setup(x => x.DependencySets)
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new Version(20, 0)), new[]
                            {
                                new PackageDependency("MyApp.Api", new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion("2.0.0")))
                            })
                        };
                    });

                var validator = new PackageTargetFrameworkValidator(defaultNuGetFrameworkMock.Object);

                var validationContext = validator.Validate(packageSearchMetadataMock.Object);

                Assert.That(validationContext.HasErrors, Is.True);

                await Verifier.Verify(validationContext.GetValidations());
            }

            [Test]
            public async Task Returns_No_Error_Without_Dependencies_Async()
            {
                var defaultNuGetFrameworkMock = new Mock<IDefaultNuGetFramework>();
                defaultNuGetFrameworkMock.Setup(x => x.GetHighest())
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new NuGetFramework(".NETFramework", new Version(4, 7, 2)),
                            new NuGetFramework(".NETCoreApp", new Version(8, 0))
                        };
                    });

                var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
                packageSearchMetadataMock.Setup(x => x.DependencySets)
                    .Returns(() =>
                    {
                        return Array.Empty<PackageDependencyGroup>();
                    });

                var validator = new PackageTargetFrameworkValidator(defaultNuGetFrameworkMock.Object);

                var validationContext = validator.Validate(packageSearchMetadataMock.Object);

                Assert.That(validationContext.HasErrors, Is.False);
            }

            [Test]
            public async Task Returns_No_Error_When_TargetFramework_Is_Not_Too_High_Async()
            {
                var defaultNuGetFrameworkMock = new Mock<IDefaultNuGetFramework>();
                defaultNuGetFrameworkMock.Setup(x => x.GetHighest())
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new NuGetFramework(".NETFramework", new Version(4, 7, 2)),
                            new NuGetFramework(".NETCoreApp", new Version(8, 0))
                        };
                    });

                var packageSearchMetadataMock = new Mock<IPackageSearchMetadata>();
                packageSearchMetadataMock.Setup(x => x.DependencySets)
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new Version(8, 0)), new[]
                            {
                                new PackageDependency("MyApp.Api", new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion("2.0.0")))
                            })
                        };
                    });

                var validator = new PackageTargetFrameworkValidator(defaultNuGetFrameworkMock.Object);

                var validationContext = validator.Validate(packageSearchMetadataMock.Object);

                Assert.That(validationContext.HasErrors, Is.False);
            }
        }
    }
}
