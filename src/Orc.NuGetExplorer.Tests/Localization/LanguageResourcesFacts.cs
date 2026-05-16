namespace Orc.NuGetExplorer.Tests;

using Catel.Services;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

public class LanguageResourcesFacts
{
    [TestFixture]
    public class The_GetRequiredString_Method
    {
        [TestCase("NuGetExplorer_WindowsCredentialProvider_WindowTitle", "Credentials required")]
        [TestCase("NuGetExplorer_DependenciesView_Label_Content_Dependencies", "Dependencies:")]
        [TestCase("NuGetExplorer_ExplorerTopBarViewModel_Message_ClearCachesPrompt", "Clean all NuGet caches, including global packages folder?")]
        public void Returns_Extracted_Resource_Value(string resourceName, string expectedValue)
        {
            var serviceCollection = ServiceCollectionHelper.CreateServiceCollection();
            using var serviceProvider = serviceCollection.BuildServiceProvider();
            var languageService = serviceProvider.GetRequiredService<ILanguageService>();

            var resourceValue = languageService.GetRequiredString(resourceName);

            Assert.That(resourceValue, Is.EqualTo(expectedValue));
        }
    }
}
