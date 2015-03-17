// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUpdatesSearcherService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackagesUpdatesSearcherService
    {
        #region Methods
        IEnumerable<IPackageDetails> SearchForUpdates(bool allowPrerelease = false, bool authenticateIfRequired = false);
        #endregion
    }
}