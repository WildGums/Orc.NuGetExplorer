namespace Orc.NuGetExplorer.Tests.Management
{
    using System.Threading;
    using System.Threading.Tasks;
    using Catel.Services;
    using Moq;
    using NuGet.Packaging.Core;
    using NUnit.Framework;
    using Orc.NuGetExplorer.Management;
    using Orc.NuGetExplorer.Services;

    public partial class NuGetProjectPackageManagerFacts
    {
        [TestFixture]
        public class The_InstallPackageForProjectAsync_Method
        {
            [Test]
            public async Task Correctly_Maps_Project_Properties_To_Context_Async()
            {
                var packageInstallationServiceMock = new Mock<IPackageInstallationService>();
                var nuGetProjectContextProviderMock = new Mock<INuGetProjectContextProvider>();
                var nuGetProjectConfigurationProvider = new Mock<INuGetProjectConfigurationProvider>();
                var messageServiceMock = new Mock<IMessageService>();
                var fileSystemServiceMock = new Mock<IFileSystemService>();
                var languageServiceMock = new Mock<ILanguageService>();

                using var nuGetProjectManagement = new TestNuGetProjectPackageManager(packageInstallationServiceMock.Object,
                    nuGetProjectContextProviderMock.Object,
                    nuGetProjectConfigurationProvider.Object,
                    messageServiceMock.Object,
                    fileSystemServiceMock.Object,
                    languageServiceMock.Object);

                var projectMock = new Mock<IExtensibleProject>();
                projectMock.Setup(x => x.IgnoreMissingDependencies)
                    .Returns(true);

                await nuGetProjectManagement.InstallPackageForProjectAsync(projectMock.Object, new PackageIdentity("SomePackage", new NuGet.Versioning.NuGetVersion("1.0.0")),
                    null, default, true);

                var context = nuGetProjectManagement.ReceivedContext;

                Assert.That(context.IgnoreMissingPackages, Is.True);
            }

            private class TestNuGetProjectPackageManager : NuGetProjectPackageManager
            {
                public TestNuGetProjectPackageManager(IPackageInstallationService packageInstallationService, INuGetProjectContextProvider nuGetProjectContextProvider, 
                    INuGetProjectConfigurationProvider nuGetProjectConfigurationProvider, IMessageService messageService, IFileSystemService fileSystemService, 
                    ILanguageService languageService) 
                    : base(packageInstallationService, nuGetProjectContextProvider, nuGetProjectConfigurationProvider, messageService, fileSystemService, languageService)
                {
                }

                public PackageInstallationContext? ReceivedContext { get; private set; }

                public override async Task<bool> InstallPackageForProjectAsync(PackageInstallationContext context, CancellationToken token)
                {
                    ReceivedContext = context;

                    return true;
                }
            }
        }
    }
}
