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
        
        public ExplorerSettingsContainerModelProvider(ITypeFactory typeFactory, INuGetConfigurationService nugetConfigurationService) : base(typeFactory)
        {
            Argument.IsNotNull(() => nugetConfigurationService);
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
            _nugetConfigurationService = nugetConfigurationService;

            _explorerSettings = new Lazy<ExplorerSettingsContainer>(() => Create());
        }

        public override ExplorerSettingsContainer Model 
        { 
            get
            {
                if (!_explorerSettings.IsValueCreated)
                {
                    base.Model = _explorerSettings.Value;
                    IsInitialized = true;
                }


                if (!IsInitialized)
                {
                    var currentValue = base.Model;
                    currentValue.Clear();
                    base.Model = ModelInitialize(base.Model);
                    IsInitialized = true;
                }


                return base.Model;
            }
            set
            {
                base.Model = value;
            }
        }

        public bool IsInitialized { get; set; }

        public override ExplorerSettingsContainer Create()
        {
            return ModelInitialize(base.Create());
        }

        private ExplorerSettingsContainer ModelInitialize(ExplorerSettingsContainer value)
        {
            var feeds = _nugetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();

            feeds.ForEach(feed => feed.Initialize());

            value.NuGetFeeds.AddRange(feeds);

            return value;
        }
    }
}
