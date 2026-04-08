namespace Orc.NuGetExplorer.Tests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using NUnit.Framework;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Providers;

public partial class PackageMetadataProviderFacts
{
    [TestFixture]
    public class The_GetHighestPackageMetadataAsync_Method
    {
        [Test]
        public async Task Respects_Specified_Predicate_Async()
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();

            // Mock registration
            var nugetConfigurationServiceMock = new Mock<INuGetConfigurationService>();
            nugetConfigurationServiceMock.Setup(x => x.GetDestinationFolder())
                .Returns(() => Environment.CurrentDirectory);
            nugetConfigurationServiceMock.Setup(x => x.LoadPackageSources())
                .Returns(() => Array.Empty<IPackageSource>());

            serviceCollection.AddSingleton<INuGetConfigurationService>(nugetConfigurationServiceMock.Object);

            var mockProject = GlobalMocks.CreateMockProject();
            var projectProvider = new Mock<IDefaultExtensibleProjectProvider>();
            projectProvider.Setup(x => x.GetDefaultProject())
                .Returns(() => mockProject);

            serviceCollection.AddSingleton<IDefaultExtensibleProjectProvider>(projectProvider.Object);

            using var serviceProvider = serviceCollection.BuildServiceProvider();

            // IRepositoryService setup
            var projectRepository = GlobalMocks.CreateMockRepository("Mock", "packages");

            var repositoryServiceMock = new Mock<IRepositoryService>();
            repositoryServiceMock.Setup(x => x.LocalRepository)
                .Returns(projectRepository);

            var repositoryService = repositoryServiceMock.Object;

            var metadataResourceMock = new Mock<PackageMetadataResource>();
            metadataResourceMock.Setup(x => x.GetMetadataAsync(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<SourceCacheContext>(), It.IsAny<ILogger>(), It.IsAny<CancellationToken>()))
                .Returns<string, bool, bool, SourceCacheContext, ILogger, CancellationToken>(async (packageId, includePrerelease, includeUnlisted, sourceCacheContext, logger, ct) =>
                {
                    var packages = new List<IPackageSearchMetadata>();

                    var package1Mock = new Mock<IPackageSearchMetadata>();
                    package1Mock.Setup(x => x.Identity)
                        .Returns(() => new PackageIdentity("MyPackage", new NuGetVersion(2, 0, 0)));
                    package1Mock.Setup(x => x.DependencySets)
                        .Returns(() =>
                        {
                            var packageDependencyGroup = new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework("net8.0"),
                                new[]
                                {
                                    new PackageDependency("SomePackage", new VersionRange(new NuGetVersion(2, 0, 0)))
                                });

                            return new[] { packageDependencyGroup };
                        });

                    var package2Mock = new Mock<IPackageSearchMetadata>();
                    package2Mock.Setup(x => x.Identity)
                        .Returns(() => new PackageIdentity("MyPackage", new NuGetVersion(1, 5, 0)));
                    package2Mock.Setup(x => x.DependencySets)
                        .Returns(() =>
                        {
                            var packageDependencyGroup = new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework("net8.0"),
                                new[]
                                {
                                    new PackageDependency("SomePackage", new VersionRange(new NuGetVersion(1, 0, 0)))
                                });

                            return new[] { packageDependencyGroup };
                        });

                    var package3Mock = new Mock<IPackageSearchMetadata>();
                    package3Mock.Setup(x => x.Identity)
                        .Returns(() => new PackageIdentity("MyPackage", new NuGetVersion(1, 0, 0)));
                    package3Mock.Setup(x => x.DependencySets)
                        .Returns(() =>
                        {
                            var packageDependencyGroup = new PackageDependencyGroup(new NuGet.Frameworks.NuGetFramework("net8.0"),
                                new[]
                                {
                                    new PackageDependency("NoPackage", new VersionRange(new NuGetVersion(1, 0, 0)))
                                });

                            return new[] { packageDependencyGroup };
                        });

                    packages.Add(package1Mock.Object);
                    packages.Add(package2Mock.Object);
                    packages.Add(package3Mock.Object);

                    return packages;
                });

            var sourceRepositoryMock = new Mock<SourceRepository>();
            sourceRepositoryMock.Setup(x => x.GetResourceAsync<PackageMetadataResource>(It.IsAny<CancellationToken>()))
                .Returns<CancellationToken>(async (ct) =>
                {
                    return metadataResourceMock.Object;
                });

            var repositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            repositoryProviderMock.Setup(x => x.CreateRepository(It.IsAny<PackageSource>()))
                .Returns<PackageSource>(ps => sourceRepositoryMock.Object);
            repositoryProviderMock.Setup(x => x.CreateRepository(It.IsAny<PackageSource>(), It.IsAny<FeedType>()))
                .Returns<PackageSource, FeedType>((ps, t) => sourceRepositoryMock.Object);

            var repositoryProvider = repositoryProviderMock.Object;

            // Create tested instance
            var packageMetadataProvider = ActivatorUtilities.CreateInstance<PackageMetadataProvider>(serviceProvider,
                new object[]
                {
                    new[] { sourceRepositoryMock.Object },
                    Array.Empty<SourceRepository>()
                });

            var package = await packageMetadataProvider.GetHighestPackageMetadataAsync("MyPackage", true,
                p => p.DependencySets.Any(pdg => !pdg.Packages.Any(d => d.Id.Contains("SomePackage"))), default);

            // Because of predicate, 1.0.0 should be chosen
            Assert.That(package, Is.Not.Null);
            Assert.That(package.Identity.Version, Is.EqualTo(new NuGetVersion(1, 0, 0)));
        }
    }
}
