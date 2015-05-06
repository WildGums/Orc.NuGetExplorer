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

    public static class INuGetConfigurationServiceExtensions
    {
        #region Methods
        public static async Task<string> GetDestinationFolderAsync(this INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return await Task.Factory.StartNew(() => nuGetConfigurationService.GetDestinationFolder());
        }

        public static async Task SetDestinationFolderAsync(this INuGetConfigurationService nuGetConfigurationService, string value)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            await Task.Factory.StartNew(() => nuGetConfigurationService.SetDestinationFolder(value));
        }

        public static async Task<IEnumerable<IPackageSource>> LoadPackageSourcesAsync(this INuGetConfigurationService nuGetConfigurationService)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return await Task.Factory.StartNew(() => nuGetConfigurationService.LoadPackageSources());
        }

        public static async Task<bool> SavePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source, bool isEnabled = true, bool isOfficial = true)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            return await Task.Factory.StartNew(() => nuGetConfigurationService.SavePackageSource(name, source, isEnabled, isOfficial));
        }

        [Obsolete("Use DisablePackageSourceAsync")]
        public static async Task DeletePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            await Task.Factory.StartNew(() => nuGetConfigurationService.DisablePackageSource(name, source));
        }

        public static async Task DisablePackageSourceAsync(this INuGetConfigurationService nuGetConfigurationService, string name, string source)
        {
            Argument.IsNotNull(() => nuGetConfigurationService);

            await Task.Factory.StartNew(() => nuGetConfigurationService.DisablePackageSource(name, source));
        }
        #endregion
    }
}