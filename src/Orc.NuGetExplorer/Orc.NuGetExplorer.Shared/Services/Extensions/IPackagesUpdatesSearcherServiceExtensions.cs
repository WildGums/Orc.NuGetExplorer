// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUpdatesSearcherServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;

    public static class IPackagesUpdatesSearcherServiceExtensions
    {
        #region Methods
        public static Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(this IPackagesUpdatesSearcherService packagesUpdatesSearcherService, bool? allowPrerelease = null, bool authenticateIfRequired = true)
        {
            Argument.IsNotNull(() => packagesUpdatesSearcherService);

            return TaskHelper.Run(() => packagesUpdatesSearcherService.SearchForUpdates(allowPrerelease, authenticateIfRequired));
        }
        #endregion
    }
}