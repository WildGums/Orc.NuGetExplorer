namespace Orc.NuGetExplorer.Tests.Services;

using System.Threading.Tasks;
using Catel.Services;
using Moq;
using NUnit.Framework;
using Orc.NuGetExplorer.Packaging;

public class PackageCommandServiceFacts
{
    [TestFixture]
    public class TheCanInstallAsyncMethod : TestFixtureBase
    {
        [TestCase]
        public async Task Returns_True_For_Not_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = false
            };

            var canInstall = await service.CanInstallAsync(packageDetails);

            Assert.That(canInstall, Is.True);
        }

        [TestCase]
        public async Task Returns_False_For_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = true
            };

            var canInstall = await service.CanInstallAsync(packageDetails);

            Assert.That(canInstall, Is.False);
        }
    }

    [TestFixture]
    public class TheCanUpdateAsyncMethod : TestFixtureBase
    {
        [TestCase]
        public async Task Returns_False_For_Not_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = false
            };

            var canUpdate = await service.CanUpdateAsync(packageDetails);

            Assert.That(canUpdate, Is.False);
        }

        [TestCase]
        public async Task Returns_True_For_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = true
            };

            var canUpdate = await service.CanUpdateAsync(packageDetails);

            Assert.That(canUpdate, Is.True);
        }
    }

    [TestFixture]
    public class TheVerifyLocalPackageExistsAsyncMethod : TestFixtureBase
    {
        [TestCase]
        public async Task Returns_False_For_Not_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = false
            };

            var isInstalled = await service.VerifyLocalPackageExistsAsync(packageDetails);

            Assert.That(isInstalled, Is.False);
        }

        [TestCase]
        public async Task Returns_True_For_Installed_Package_Async()
        {
            var busyIndicatorServiceMock = new Mock<IBusyIndicatorService>();
            var repositoryServiceMock = new Mock<IRepositoryService>();
            var packageQueryServiceMock = new Mock<IPackageQueryService>();
            var packageOperationServiceMock = new Mock<IPackageOperationService>();
            var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
            var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

            var service = new PackageCommandService(busyIndicatorServiceMock.Object, repositoryServiceMock.Object,
                packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

            var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
            var packageDetails = new PackageDetails(packageSearchMetadata)
            {
                IsInstalled = true
            };

            var isInstalled = await service.VerifyLocalPackageExistsAsync(packageDetails);

            Assert.That(isInstalled, Is.True);
        }
    }
}
