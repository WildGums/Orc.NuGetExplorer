namespace Orc.NuGetExplorer.Providers
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Configuration;
    using Orc.NuGetExplorer.Models;

    /// <summary>
    /// Provider that retrive one pre-configured package source saved under unique key
    /// </summary>
    public class FallbackSourceDefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        private readonly string _fallbackSourceKey;
        private readonly IConfigurationService _configurationService;

        public FallbackSourceDefaultPackageSourcesProvider(string fallbackSourceKey, IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            _fallbackSourceKey = fallbackSourceKey;
            _configurationService = configurationService;
        }

        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            if (string.IsNullOrEmpty(_fallbackSourceKey))
            {
                yield break;
            }

            var configuredUri = _configurationService.GetRoamingValue<string>(_fallbackSourceKey);

            if(string.IsNullOrEmpty(configuredUri))
            {
                yield break;
            }

            var packageSource = new NuGetFeed("Plugins", configuredUri, true);

            yield return packageSource;
        }
    }
}
