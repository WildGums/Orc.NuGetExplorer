namespace Orc.NuGetExplorer.Tests.Services
{
    using System.Threading.Tasks;
    using Catel.Services;
    using NUnit.Framework;

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
    }
}
