namespace Orc.NuGetExplorer.Tests
{
    using Catel;
    using Microsoft.Extensions.DependencyInjection;
    using Orc.Controls;
    using Orc.FileSystem;

    internal static class ServiceCollectionHelper
    {
        public static IServiceCollection CreateServiceCollection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddLogging();
            serviceCollection.AddCatelCore();
            serviceCollection.AddCatelMvvm();
            serviceCollection.AddOrcControls();
            serviceCollection.AddOrcFileSystem();
            serviceCollection.AddOrcNuGetExplorer();
            serviceCollection.AddOrcNuGetExplorerXaml();

            return serviceCollection;
        }
    }
}
