// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Configuration;

    internal static class IConfigurationServiceExtensions
    {
        public static string GetLastRepositoryCategory(this IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);

            var value = configurationService.GetRoamingValue(AppSettings.NuGetExplorer.LastRepositoryCaregory, AppSettings.NuGetExplorer.LastRepositoryCaregoryDefaultValue);

            return value;
        }

        public static void SetLastRepositoryCategory(this IConfigurationService configurationService, string value)
        {
            Argument.IsNotNull(() => configurationService);

            configurationService.SetRoamingValue(AppSettings.NuGetExplorer.LastRepositoryCaregory, value);
        }

        public static string GetLastRepository(this IConfigurationService configurationService, string repositoryCategory)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => repositoryCategory);

            var key = GetLastRepositoryKey(repositoryCategory);
            var value = configurationService.GetRoamingValue(key, AppSettings.NuGetExplorer.LastRepositoryDefaultValue);

            return value;
        }

        public static void SetLastRepository(this IConfigurationService configurationService, string page, IRepository repository)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => repository);

            var key = GetLastRepositoryKey(page);
            configurationService.SetRoamingValue(key, repository.Name);
        }

        public static void SetLastRepository(this IConfigurationService configurationService, string page, string repositoryName)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => repositoryName);

            var key = GetLastRepositoryKey(page);
            configurationService.SetRoamingValue(key, repositoryName);
        }

        public static bool GetIsPrereleaseIncluded(this IConfigurationService configurationService)
        {
            Argument.IsNotNull(() => configurationService);
            return configurationService.GetRoamingValue<bool>(Settings.NuGet.IncludePrereleasePackages, false);
        }

        public static void SetIsPrereleaseIncluded(this IConfigurationService configurationService, bool isPrereleaseIncluded)
        {
            Argument.IsNotNull(() => configurationService);
            configurationService.SetRoamingValue(Settings.NuGet.IncludePrereleasePackages, isPrereleaseIncluded);
        }

        public static void SetIsHideInstalled(this IConfigurationService configurationService, bool isHideInstalled)
        {
            Argument.IsNotNull(() => configurationService);
            configurationService.SetRoamingValue(Settings.NuGet.HideInstalledPackages, isHideInstalled);
        }

        private static string GetLastRepositoryKey(string repositoryCategory)
        {
            return string.Format("{0}.{1}", AppSettings.NuGetExplorer.LastRepository, repositoryCategory);
        }
    }
}
