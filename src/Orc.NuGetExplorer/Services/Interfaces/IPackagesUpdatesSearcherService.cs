// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUpdatesSearcherService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPackagesUpdatesSearcherService
    {
        #region Methods
        IEnumerable<IPackageDetails> SearchForUpdates(bool? allowPrerelease = null, bool authenticateIfRequired = true);
        Task<IEnumerable<IPackageDetails>> SearchForUpdatesAsync(CancellationToken token, bool? allowPrerelease = null, bool authenticateIfRequired = true);
        #endregion
    }
}