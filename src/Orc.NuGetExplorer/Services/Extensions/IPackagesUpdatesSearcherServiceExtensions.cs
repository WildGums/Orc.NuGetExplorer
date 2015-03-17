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

    public static class IPackagesUpdatesSearcherServiceExtensions
    {
        public static async Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(this IPackagesUpdatesSearcherService packagesUpdatesSearcherService, bool allowPrerelease, bool authenticateIfRequired = true)
        {
            Argument.IsNotNull(() => packagesUpdatesSearcherService);

            return await Task.Factory.StartNew(() => packagesUpdatesSearcherService.SearchForUpdates(allowPrerelease, authenticateIfRequired));
        }
    }
}