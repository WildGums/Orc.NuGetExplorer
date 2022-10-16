namespace Orc.NuGetExplorer
{
    using Catel.Configuration;

    internal static class IConfigurationServiceExtensions
    {
        public static string GetLastRepositoryCategory(this IConfigurationService configurationService)
        {
            ArgumentNullException.ThrowIfNull(configurationService);

            var value = configurationService.GetRoamingValue(AppSettings.NuGetExplorer.LastRepositoryCaregory, AppSettings.NuGetExplorer.LastRepositoryCaregoryDefaultValue);

            return value;
        }

        public static void SetLastRepositoryCategory(this IConfigurationService configurationService, string value)
        {
            ArgumentNullException.ThrowIfNull(configurationService);

            configurationService.SetRoamingValue(AppSettings.NuGetExplorer.LastRepositoryCaregory, value);
        }

        public static string GetLastRepository(this IConfigurationService configurationService, string repositoryCategory)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(repositoryCategory);

            var key = GetLastRepositoryKey(repositoryCategory);
            var value = configurationService.GetRoamingValue(key, AppSettings.NuGetExplorer.LastRepositoryDefaultValue);

            return value;
        }

        public static void SetLastRepository(this IConfigurationService configurationService, string page, IRepository repository)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(repository);

            var key = GetLastRepositoryKey(page);
            configurationService.SetRoamingValue(key, repository.Name);
        }

        public static void SetLastRepository(this IConfigurationService configurationService, string page, string repositoryName)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            ArgumentNullException.ThrowIfNull(repositoryName);

            var key = GetLastRepositoryKey(page);
            configurationService.SetRoamingValue(key, repositoryName);
        }

        public static bool GetIsPrereleaseIncluded(this IConfigurationService configurationService)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            return configurationService.GetRoamingValue(Settings.NuGet.IncludePrereleasePackages, false);
        }

        public static void SetIsPrereleaseIncluded(this IConfigurationService configurationService, bool isPrereleaseIncluded)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            configurationService.SetRoamingValue(Settings.NuGet.IncludePrereleasePackages, isPrereleaseIncluded);
        }

        public static void SetIsHideInstalled(this IConfigurationService configurationService, bool isHideInstalled)
        {
            ArgumentNullException.ThrowIfNull(configurationService);
            configurationService.SetRoamingValue(Settings.NuGet.HideInstalledPackages, isHideInstalled);
        }

        private static string GetLastRepositoryKey(string repositoryCategory)
        {
            return string.Format("{0}.{1}", AppSettings.NuGetExplorer.LastRepository, repositoryCategory);
        }
    }
}
