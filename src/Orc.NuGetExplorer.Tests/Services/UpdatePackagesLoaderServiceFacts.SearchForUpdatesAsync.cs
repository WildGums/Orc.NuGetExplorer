namespace Orc.NuGetExplorer.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Pagination;
    using Orc.NuGetExplorer.Providers;

    public partial class UpdatePackagesLoaderServiceFacts
    {
        [TestFixture]
        public class The_SearchForUpdatesAsync_Method
        {
            [Test]
            public async Task Returns_EmptyList_When_NoUpdatesAvailable_Async()
            {
                // Arrange
                var localRepositoryMock = CreateLocalRepositoryMock();

                var repositoryServiceMock = new Mock<IRepositoryService>();
                repositoryServiceMock.Setup(x => x.LocalRepository)
                    .Returns(localRepositoryMock.Object);

                var projectLocatorMock = new Mock<IExtensibleProjectLocator>();
                var packageManagerMock = new Mock<INuGetPackageManager>();
                var packageValidatorProviderMock = new Mock<IPackageValidatorProvider>();
                packageValidatorProviderMock.Setup(x => x.GetValidators())
                    .Returns(() => Array.Empty<IPackageValidator>());

                var projectRepositoryLoaderMock = new Mock<IPackageLoaderService>();
                var metadataProviderMock = new Mock<IPackageMetadataProvider>();

                projectRepositoryLoaderMock.Setup(x => x.PackageMetadataProvider)
                    .Returns(metadataProviderMock.Object);

                var service = new UpdatePackagesLoaderService(
                    repositoryServiceMock.Object,
                    projectLocatorMock.Object,
                    packageManagerMock.Object,
                    packageValidatorProviderMock.Object);

                // Inject the mocked project repository loader
                service.GetType()
                    .GetField("_projectRepositoryLoader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(service, new Lazy<IPackageLoaderService>(() => projectRepositoryLoaderMock.Object));

                var packageMock = new Mock<IPackageSearchMetadata>();
                packageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("1.0.0")));

                var installedPackages = new List<IPackageSearchMetadata>
                {
                    packageMock.Object
                };

                var availablePackageMock = new Mock<IPackageSearchMetadata>();
                availablePackageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("1.0.0")));

                var availablePackages = new List<IPackageSearchMetadata>
                {
                    availablePackageMock.Object
                };

                projectRepositoryLoaderMock
                    .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<PageContinuation>(), It.IsAny<SearchFilter>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(installedPackages);

                metadataProviderMock
                    .Setup(x => x.GetHighestPackageMetadataAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IPackageSearchMetadata, bool>>(), It.IsAny<CancellationToken>()))
                    .Returns<string, bool, Func<IPackageSearchMetadata, bool>, CancellationToken>(async (s, b, p, ct) =>
                    {
                        if (p(availablePackageMock.Object))
                        {
                            return availablePackageMock.Object;
                        }

                        return null;
                    });

                // Act
                var result = await service.SearchForUpdatesAsync();

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.Empty);
            }

            [Test]
            public async Task Returns_Updates_When_ValidUpdatesAvailable_Async()
            {
                // Arrange
                var localRepositoryMock = CreateLocalRepositoryMock();

                var repositoryServiceMock = new Mock<IRepositoryService>();
                repositoryServiceMock.Setup(x => x.LocalRepository)
                    .Returns(localRepositoryMock.Object);

                var projectLocatorMock = new Mock<IExtensibleProjectLocator>();
                var packageManagerMock = new Mock<INuGetPackageManager>();
                var packageValidatorProviderMock = new Mock<IPackageValidatorProvider>();
                packageValidatorProviderMock.Setup(x => x.GetValidators())
                    .Returns(() => Array.Empty<IPackageValidator>());

                var projectRepositoryLoaderMock = new Mock<IPackageLoaderService>();
                var metadataProviderMock = new Mock<IPackageMetadataProvider>();

                projectRepositoryLoaderMock.Setup(x => x.PackageMetadataProvider)
                    .Returns(metadataProviderMock.Object);

                var service = new UpdatePackagesLoaderService(
                    repositoryServiceMock.Object,
                    projectLocatorMock.Object,
                    packageManagerMock.Object,
                    packageValidatorProviderMock.Object);

                // Inject the mocked project repository loader
                service.GetType()
                    .GetField("_projectRepositoryLoader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(service, new Lazy<IPackageLoaderService>(() => projectRepositoryLoaderMock.Object));

                var installedPackageMock = new Mock<IPackageSearchMetadata>();
                installedPackageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("1.0.0")));

                var installedPackages = new List<IPackageSearchMetadata>
                {
                    installedPackageMock.Object
                };

                var availablePackageMock = new Mock<IPackageSearchMetadata>();
                availablePackageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("2.0.0")));
                availablePackageMock.Setup(x => x.DependencySets)
                    .Returns(new[]
                    {
                        new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new Version(8, 0)), new[]
                        {
                            new PackageDependency("MyApp.Api", new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion("2.0.0")))
                        })
                    });

                var availablePackages = new List<IPackageSearchMetadata>
                {
                    availablePackageMock.Object
                };

                projectRepositoryLoaderMock
                    .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<PageContinuation>(), It.IsAny<SearchFilter>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(installedPackages);

                metadataProviderMock
                    .Setup(x => x.GetHighestPackageMetadataAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IPackageSearchMetadata, bool>>(), It.IsAny<CancellationToken>()))
                    .Returns<string, bool, Func< IPackageSearchMetadata, bool>, CancellationToken>(async (s, b, p, ct) =>
                    {
                        if (p(availablePackageMock.Object))
                        {
                            return availablePackageMock.Object;
                        }

                        return null;
                    });

                // Act
                var result = await service.SearchForUpdatesAsync();

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(1));
                Assert.That(result.First().GetIdentity().Id, Is.EqualTo("TestPackage"));
                Assert.That(result.First().GetIdentity().Version.ToString(), Is.EqualTo("2.0.0"));
            }

            [Test]
            public async Task Returns_No_Updates_When_InvalidUpdatesAvailable_Async()
            {
                // Arrange
                var localRepositoryMock = CreateLocalRepositoryMock();

                var repositoryServiceMock = new Mock<IRepositoryService>();
                repositoryServiceMock.Setup(x => x.LocalRepository)
                    .Returns(localRepositoryMock.Object);

                var projectLocatorMock = new Mock<IExtensibleProjectLocator>();
                var packageManagerMock = new Mock<INuGetPackageManager>();
                var packageValidatorProviderMock = new Mock<IPackageValidatorProvider>();
                packageValidatorProviderMock.Setup(x => x.GetValidators())
                    .Returns(() =>
                    {
                        return new[]
                        {
                            new PackageTargetFrameworkValidator(CreateDefaultNuGetFrameworkMock().Object)
                        };
                    });

                var projectRepositoryLoaderMock = new Mock<IPackageLoaderService>();
                var metadataProviderMock = new Mock<IPackageMetadataProvider>();

                projectRepositoryLoaderMock.Setup(x => x.PackageMetadataProvider)
                    .Returns(metadataProviderMock.Object);

                var service = new UpdatePackagesLoaderService(
                    repositoryServiceMock.Object,
                    projectLocatorMock.Object,
                    packageManagerMock.Object,
                    packageValidatorProviderMock.Object);

                // Inject the mocked project repository loader
                service.GetType()
                    .GetField("_projectRepositoryLoader", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(service, new Lazy<IPackageLoaderService>(() => projectRepositoryLoaderMock.Object));

                var installedPackageMock = new Mock<IPackageSearchMetadata>();
                installedPackageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("1.0.0")));

                var installedPackages = new List<IPackageSearchMetadata>
                {
                    installedPackageMock.Object
                };

                var availablePackageMock = new Mock<IPackageSearchMetadata>();
                availablePackageMock.Setup(x => x.Identity)
                    .Returns(new NuGet.Packaging.Core.PackageIdentity("TestPackage", new NuGet.Versioning.NuGetVersion("2.0.0")));
                availablePackageMock.Setup(x => x.DependencySets)
                    .Returns(new[]
                    {
                        new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework(".NETCoreApp", new Version(20, 0)), new[]
                        {
                            new PackageDependency("MyApp.Api", new NuGet.Versioning.VersionRange(new NuGet.Versioning.NuGetVersion("2.0.0")))
                        })
                    });

                var availablePackages = new List<IPackageSearchMetadata>
                {
                    availablePackageMock.Object
                };

                projectRepositoryLoaderMock
                    .Setup(x => x.LoadAsync(It.IsAny<string>(), It.IsAny<PageContinuation>(), It.IsAny<SearchFilter>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(installedPackages);

                metadataProviderMock
                    .Setup(x => x.GetHighestPackageMetadataAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<Func<IPackageSearchMetadata, bool>>(), It.IsAny<CancellationToken>()))
                    .Returns<string, bool, Func<IPackageSearchMetadata, bool>, CancellationToken>(async (s, b, p, ct) =>
                    {
                        if (p(availablePackageMock.Object))
                        {
                            return availablePackageMock.Object;
                        }

                        return null;
                    });

                // Act
                var result = await service.SearchForUpdatesAsync();

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Count(), Is.EqualTo(0));
            }
        }
    }
}
