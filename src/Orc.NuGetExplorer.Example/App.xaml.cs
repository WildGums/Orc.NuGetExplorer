namespace Orc.NuGetExplorer.Example
{
    using System.Globalization;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Catel.Services;
    using Orc.NuGetExplorer.Services;
    using Orc.Theming;
    using Orchestra;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
#if DEBUG
            LogManager.AddDebugListener(true);
#endif
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var languageService = ServiceLocator.Default.ResolveType<ILanguageService>();

            // Note: it's best to use .CurrentUICulture in actual apps since it will use the preferred language
            // of the user. But in order to demo multilingual features for devs (who mostly have en-US as .CurrentUICulture),
            // we use .CurrentCulture for the sake of the demo
            languageService.PreferredCulture = CultureInfo.CurrentCulture;
            languageService.FallbackCulture = new CultureInfo("en-US");

            this.ApplyTheme();

            base.OnStartup(e);
        }

        [ModuleInitializer]
        public static async void InitializeAsync()
        {
            var serviceLocator = ServiceLocator.Default;

            serviceLocator.RegisterType<IEchoService, EchoService>();
            serviceLocator.RegisterType<IDefaultPackageSourcesProvider, DefaultPackageSourcesProvider>();

            serviceLocator.RegisterType<INuGetExplorerInitializationService, ExampleNuGetExplorerInitializationService>();
            serviceLocator.RegisterType<INuGetLogListeningSevice, NoVerboseHttpNuGetLogListeningService>();

            var configurationService = serviceLocator.ResolveRequiredType<IConfigurationService>();
            await configurationService.LoadAsync();
        }
    }
}
