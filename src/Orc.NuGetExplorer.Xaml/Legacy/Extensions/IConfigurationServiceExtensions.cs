// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationServiceExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
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

        public static string GetLastRepository(this IConfigurationService configurationService, RepositoryCategory repositoryCategory)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => repositoryCategory);

            var key = GetLastRepositoryKey(repositoryCategory);
            var value = configurationService.GetRoamingValue(key, AppSettings.NuGetExplorer.LastRepositoryDefaultValue);

            return value;
        }

        public static void SetLastRepository(this IConfigurationService configurationService, RepositoryCategory repositoryCategory, IRepository repository)
        {
            Argument.IsNotNull(() => configurationService);
            Argument.IsNotNull(() => repositoryCategory);
            Argument.IsNotNull(() => repository);

            var key = GetLastRepositoryKey(repositoryCategory);
            configurationService.SetRoamingValue(key, repository.Name);
        }

        private static string GetLastRepositoryKey(RepositoryCategory repositoryCategory)
        {
            return string.Format("{0}.{1}", AppSettings.NuGetExplorer.LastRepository, repositoryCategory.Name);
        }
    }
}