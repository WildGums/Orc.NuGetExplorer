namespace Orc.NuGetExplorer.Providers
{
    using System;
    using System.Linq;
    using Catel.Configuration;
    using Catel.IoC;
    using Catel.Logging;
    using Orc.NuGetExplorer.Models;

    public class ExplorerSettingsContainerModelProvider : ModelProvider<ExplorerSettingsContainer>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly INuGetConfigurationService _nugetConfigurationService;
        private readonly IConfigurationService _configurationService;
        private readonly Lazy<ExplorerSettingsContainer> _explorerSettings;

        public ExplorerSettingsContainerModelProvider(ITypeFactory typeFactory, INuGetConfigurationService nugetConfigurationService, IConfigurationService configurationService)
            : base(typeFactory)
        {
            ArgumentNullException.ThrowIfNull(nugetConfigurationService);
            ArgumentNullException.ThrowIfNull(configurationService);

            _nugetConfigurationService = nugetConfigurationService;
            _configurationService = configurationService;

            _explorerSettings = new Lazy<ExplorerSettingsContainer>(() => Create());
        }

        public override ExplorerSettingsContainer? Model
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
                    if (currentValue is null)
                    {
                        throw Log.ErrorAndCreateException<InvalidOperationException>("'Model' must be non-null value");
                    }
                    currentValue.Clear();
                    base.Model = InitializeModel(currentValue);
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
            return InitializeModel(base.Create());
        }

        private ExplorerSettingsContainer InitializeModel(ExplorerSettingsContainer value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var feeds = _nugetConfigurationService.LoadPackageSources(false).OfType<NuGetFeed>().ToList();
            var prerelease = _configurationService.GetIsPrereleaseIncluded();

            feeds.ForEach(feed => feed.Initialize());

            value.NuGetFeeds.AddRange(feeds);
            value.IsPreReleaseIncluded = prerelease;

            return value;
        }
    }
}
