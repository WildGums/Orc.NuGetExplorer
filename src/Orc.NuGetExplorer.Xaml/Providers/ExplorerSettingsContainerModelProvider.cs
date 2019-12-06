namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Linq;
    using Catel;
    using Catel.IoC;
    using Orc.NuGetExplorer.Models;

    public class ExplorerSettingsContainerModelProvider : ModelProvider<ExplorerSettingsContainer>
    {
        private readonly ITypeFactory _typeFactory;
        private readonly INuGetConfigurationService _nugetConfigurationService;
        private readonly Lazy<ExplorerSettingsContainer> _explorerSettings;

        public ExplorerSettingsContainerModelProvider(ITypeFactory typeFactory, INuGetConfigurationService nugetConfigurationService)
        {
            Argument.IsNotNull(() => nugetConfigurationService);
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
            _nugetConfigurationService = nugetConfigurationService;
            _explorerSettings = new Lazy<ExplorerSettingsContainer>(() => LazyModelInitializer());
        }

        public override ExplorerSettingsContainer Model 
        { 
            get
            {
                if (!_explorerSettings.IsValueCreated)
                {
                    base.Model = _explorerSettings.Value;
                }

                return base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        private ExplorerSettingsContainer LazyModelInitializer()
        {
            var value = _typeFactory.CreateInstance<ExplorerSettingsContainer>();

            var feeds = _nugetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();

            feeds.ForEach(feed => feed.Initialize());

            value.NuGetFeeds.AddRange(feeds);

            return value;
        }
    }
}
