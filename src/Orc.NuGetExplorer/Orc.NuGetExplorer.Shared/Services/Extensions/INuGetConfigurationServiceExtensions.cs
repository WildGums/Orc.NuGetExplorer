// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetConfigurationServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;

    public static class INuGetConfigurationServiceExtensions
    {
        #region Methods
        public static Task<string> GetDestinationFolderAsync(this INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.GetDestinationFolder());
        }

        public static Task SetDestinationFolderAsync(this INuGetConfigurationService nuGetConfigurationService, string value)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.SetDestinationFolder(value));
        }

        public static Task<IEnumerable<IPackageSource>> LoadPackageSourcesAsync(this INuGetConfigurationService nuGetConfigurationService, bool onlyEnabled = false)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.LoadPackageSources(onlyEnabled));
        }

        public static Task<bool> SavePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source, bool isEnabled = true, bool isOfficial = true)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.SavePackageSource(name, source, isEnabled, isOfficial));
        }

        [Obsolete("Use DisablePackageSourceAsync")]
        public static Task DeletePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.DisablePackageSource(name, source));
        }

        public static Task DisablePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return TaskHelper.Run(() => nuGetConfigurationService.DisablePackageSource(name, source));
        }
        #endregion
    }
}