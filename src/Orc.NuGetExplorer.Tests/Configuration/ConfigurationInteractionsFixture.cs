namespace Orc.NuGetExplorer.Tests.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.IoC;
    using NuGet.Configuration;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using Orc.NuGetExplorer.Models;

    /// <summary>
    /// Test fixture that contains tests verify interactions between INuGetConfigurationService, NuGetSettings and IPackageSourceProvider from NuGet library
    /// So we sure they can be used together for achieving expected results
    /// </summary>
    [TestFixture]
    public class ConfigurationInteractionsFixture
    {
        private static readonly string TestConfigurationFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Microsoft Corporation\\Microsoft.TestHost\\configuration.xml");

        private static readonly PackageSource[] TestPackageSources =
        {
            new PackageSource("https://api.nuget.org/v3/index.json", "nuget.org", true),
            new PackageSource("C:\\LocalPackages", "local", true),
            new PackageSource("https://api.nuget.org/v3/index.json", "nuget2.org", true),
            new PackageSource("C:\\Update", "local_update", true),
        };

        private IPackageSourceProvider _packageSourceProvider;
        private INuGetConfigurationService _configurationService;

        [SetUp]
        public void ResetConfiguration()
        {
            File.Delete(TestConfigurationFilePath);
        }

        [OneTimeSetUp]
        public void InitializeServices()
        {
            var serviceLocator = ServiceLocator.Default;
            _packageSourceProvider = serviceLocator.ResolveType<IPackageSourceProvider>();
            _configurationService = serviceLocator.ResolveType<INuGetConfigurationService>();
        }

        [Test]
        public void ArePackagesSourcesCanBeSavedByDefaultProvider()
        {
            List<PackageSource> packageSources = new(TestPackageSources);

            _packageSourceProvider.SavePackageSources(packageSources);
            var configuredSources = _configurationService.LoadPackageSources(true).ToList();

            Assert.AreEqual(packageSources.Count, configuredSources.Count);

            // Disable some sources
            configuredSources[1].IsEnabled = false;
            configuredSources[3].IsEnabled = false;

            _packageSourceProvider.SavePackageSources(configuredSources.ToPackageSourceInstances());

            var disabledSourcesFromConfig = _configurationService.LoadPackageSources(true).ToList();
            Assert.AreEqual(disabledSourcesFromConfig.Count, 2);
            Assert.AreEqual(disabledSourcesFromConfig[0].IsEnabled, true);
            Assert.AreEqual(disabledSourcesFromConfig[1].IsEnabled, true);

            var allPackageSourcesFromConfig = _configurationService.LoadPackageSources(false).ToList();
            Assert.AreEqual(allPackageSourcesFromConfig.Count, 4);
            Assert.AreEqual(allPackageSourcesFromConfig[0].IsEnabled, true);
            Assert.AreEqual(allPackageSourcesFromConfig[1].IsEnabled, false);
            Assert.AreEqual(allPackageSourcesFromConfig[2].IsEnabled, true);
            Assert.AreEqual(allPackageSourcesFromConfig[3].IsEnabled, false);
        }

        [Test]
        public async Task AreNuGetSettingsCompatibilityNotBrokenAsync()
        {
            var sampleConfig = @"<?xml version=""1.0"" encoding=""utf-8""?>
  <DynamicConfiguration xmlns:ctl=""http://schemas.catelproject.com"" xmlns:i=""http://www.w3.org/2001/XMLSchema-instance"">
  <NuGetExplorer.Version ctl:type=""System.String"">4.5.4.0</NuGetExplorer.Version>
  <NuGetExplorer.MinimalVersion ctl:type=""System.String"">4.0.0</NuGetExplorer.MinimalVersion>
  <DestFolder ctl:type=""System.String"">C:\Users\JustAnotherUser\AppData\Roaming\WildGums\Gum.Ui.Example_alpha\plugins</DestFolder>
  <NuGet_sections ctl:type=""System.String"">packageSources</NuGet_sections>
  <NuGet_packageSources_values ctl:type=""System.String"">PackageSource|PackageSource2|PackageSourceLocal</NuGet_packageSources_values>
  <NuGet_packageSources_value_PackageSource ctl:type=""System.String"">https://api.nuget.org/v3/index.json</NuGet_packageSources_value_PackageSource>
  <NuGet_packageSources_value_PackageSource2 ctl:type=""System.String"">https://api.nuget.org/v3/index.json</NuGet_packageSources_value_PackageSource2>
  <NuGet_packageSources_value_PackageSourceLocal ctl:type=""System.String"">C:\Dev\_local_package_source</NuGet_packageSources_value_PackageSourceLocal>
  <NuGetExplorer.LastRepository.Browse ctl:type=""System.String"">All</NuGetExplorer.LastRepository.Browse>
  <NuGetExplorer.IncludePrerelease ctl:type=""System.String"">False</NuGetExplorer.IncludePrerelease>
</DynamicConfiguration>";

            await File.WriteAllTextAsync(TestConfigurationFilePath, sampleConfig);

            var packageSources = _packageSourceProvider.LoadPackageSources().OrderBy(x => x.Name).ToList();

            Assert.AreEqual("PackageSource", packageSources[0].Name);
            Assert.AreEqual("PackageSource2", packageSources[1].Name);
            Assert.AreEqual("PackageSourceLocal", packageSources[2].Name);
            Assert.AreEqual("https://api.nuget.org/v3/index.json", packageSources[0].Source);
            Assert.AreEqual("https://api.nuget.org/v3/index.json", packageSources[1].Source);
            Assert.AreEqual(@"C:\Dev\_local_package_source", packageSources[2].Source);
        }

        [TestCase(null, "", true)]
        [TestCase("", null, true)]
        [TestCase("", "", true)]
        [TestCase("localsource.org", "C:\\LocalPackages", false)]
        [TestCase("nuget.org", "https://api.nuget.org/v3/index.json", false)]
        [TestCase(@"""nuget.org""", "https://api.nuget.org/v3/index.json", true)] // Invalid xml character (quotations)
        [TestCase(@"C:\\LocalPackages", "C:\\LocalPackages", false)] // Slashes
        public void ConfigurationCanHandlePossiblePackageSourcesValues(string packageSourceName, string packageSourceValue, bool expectedValidationError)
        {
            var packageSource = new NuGetFeed(packageSourceName, packageSourceValue);

            packageSource.Validate();
            var HasValidationError = !packageSource.IsValid();
            Assert.AreEqual(expectedValidationError, HasValidationError);

            if (HasValidationError)
            {
                // Invalid NuGetFeed cannot be converted to PackageSourcek
                return;
            }

            Assert.DoesNotThrow(() =>
            {
                _packageSourceProvider.SavePackageSources(new[] { packageSource }.ToPackageSourceInstances());
                var savedSource = _packageSourceProvider.LoadPackageSources().FirstOrDefault(ps => ps.Name == packageSource.Name);

                Assert.IsNotNull(savedSource);
                Assert.AreEqual(packageSourceValue, savedSource.Source);
            });
        }
    }
}
