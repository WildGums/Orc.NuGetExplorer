namespace Orc.NuGetExplorer.Tests.Services
{
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
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = false;

                var canInstall = await service.CanInstallAsync(packageDetails);

                Assert.IsTrue(canInstall);
            }

            [TestCase]
            public async Task Returns_False_For_Installed_Package_Async()
            {
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = true;

                var canInstall = await service.CanInstallAsync(packageDetails);

                Assert.IsFalse(canInstall);
            }
        }

        [TestFixture]
        public class TheCanUpdateAsyncMethod : TestFixtureBase
        {
            [TestCase]
            public async Task Returns_False_For_Not_Installed_Package_Async()
            {
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = false;

                var canUpdate = await service.CanUpdateAsync(packageDetails);

                Assert.IsFalse(canUpdate);
            }

            [TestCase]
            public async Task Returns_True_For_Installed_Package_Async()
            {
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = true;

                var canUpdate = await service.CanUpdateAsync(packageDetails);

                Assert.IsTrue(canUpdate);
            }
        }

        [TestFixture]
        public class TheVerifyLocalPackageExistsAsyncMethod : TestFixtureBase
        {
            [TestCase]
            public async Task Returns_False_For_Not_Installed_Package_Async()
            {
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = false;

                var isInstalled = await service.VerifyLocalPackageExistsAsync(packageDetails);

                Assert.IsFalse(isInstalled);
            }

            [TestCase]
            public async Task Returns_True_For_Installed_Package_Async()
            {
                var pleaseWaitServiceMock = new Mock<IPleaseWaitService>();
                var repositoryServiceMock = new Mock<IRepositoryService>();
                var packageQueryServiceMock = new Mock<IPackageQueryService>();
                var packageOperationServiceMock = new Mock<IPackageOperationService>();
                var packageOperationContextServiceMock = new Mock<IPackageOperationContextService>();
                var apiPackageRegistryMock = new Mock<IApiPackageRegistry>();

                var service = new PackageCommandService(pleaseWaitServiceMock.Object, repositoryServiceMock.Object,
                    packageQueryServiceMock.Object, packageOperationServiceMock.Object,
                    packageOperationContextServiceMock.Object, apiPackageRegistryMock.Object);

                var packageSearchMetadata = CreatePackageSearchMetadata("MyPackageId", "1.0.0");
                var packageDetails = new PackageDetails(packageSearchMetadata);
                packageDetails.IsInstalled = true;

                var isInstalled = await service.VerifyLocalPackageExistsAsync(packageDetails);

                Assert.IsTrue(isInstalled);
            }
        }
    }
}
