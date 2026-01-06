namespace Orc.NuGetExplorer.Tests;

using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Catel.Linq;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NuGet.Configuration;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NUnit.Framework;
using Orc.NuGetExplorer.Management;
using Orc.NuGetExplorer.Providers;

public partial class PackageMetadataProviderFacts
{
    [TestFixture]
    public class The_GetLocalPackageMetadataAsync_Method
    {
        [TestCase("packages")]
        public async Task Can_RetrieveLocalPackage_From_CustomInstallPath_Async(string relativePath)
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

            // ISourceRepositoryProvider setup
            var providers = Repository.Provider.GetCoreV3().Select(x => x.Value).ToList();

            var repositoryProviderMock = new Mock<ISourceRepositoryProvider>();
            repositoryProviderMock.Setup(x => x.CreateRepository(It.IsAny<PackageSource>()))
                .Returns<PackageSource>(ps => new SourceRepository(ps, providers));
            repositoryProviderMock.Setup(x => x.CreateRepository(It.IsAny<PackageSource>(), It.IsAny<FeedType>()))
                .Returns<PackageSource, FeedType>((ps, t) => new SourceRepository(ps, providers));

            var repositoryProvider = repositoryProviderMock.Object;

            // Create tested instance
            var packageMetadataProvider = ActivatorUtilities.CreateInstance<PackageMetadataProvider>(serviceProvider);

            var package = new PackageIdentity("Orc.NuGetExplorer", new NuGet.Versioning.NuGetVersion("1.0.0"));
            _ = await packageMetadataProvider.GetLocalPackageMetadataAsync(package, false, System.Threading.CancellationToken.None);

            // In good case we should have initialized "_localRepository" field inside PackageMetadataProvider;
            var actualRepository = packageMetadataProvider.GetType().GetField("_localRepository", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(packageMetadataProvider) as SourceRepository;

            Assert.That(actualRepository, Is.Not.Null);
            Assert.That(actualRepository.PackageSource, Is.Not.Null);
            Assert.That(actualRepository.PackageSource.Source, Is.EqualTo($"{Environment.CurrentDirectory}\\{relativePath}"));
        }
    }
}
