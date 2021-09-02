namespace Orc.NuGetExplorer.Tests.Configuration
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel.IoC;
    using NuGet.Configuration;
    using NUnit.Framework;

    /// <summary>
    /// Test fixture that contains tests verify interactions between INuGetConfigurationService, NuGetSettings and IPackageSourceProvider from NuGet library
    /// So we sure they can be used together for achieving expected results
    /// </summary>
    [TestFixture]
    public class ConfigurationInteractionsFixture
    {
        [Test]
        public void ArePackagesSourcesCanBeSavedByProvider()
        {
            List<PackageSource> packageSources = new();

            var serviceLocator = ServiceLocator.Default;
            var packageSource = serviceLocator.ResolveType<IPackageSourceProvider>();
            var confugurationService = serviceLocator.ResolveType<INuGetConfigurationService>();

            var nuget = new PackageSource("https://api.nuget.org/v3/index.json", "nuget.org", true);
            var localFeed = new PackageSource("C:\\LocalPackages", "local", true);

            packageSources.Add(nuget);
            packageSources.Add(localFeed);

            packageSource.SavePackageSources(packageSources);
            var configuredSources = confugurationService.LoadPackageSources(true).ToList();

            Assert.AreEqual(packageSources.Count, configuredSources.Count);
        }
    }
}
