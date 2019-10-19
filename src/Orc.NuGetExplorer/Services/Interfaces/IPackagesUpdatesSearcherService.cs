// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUpdatesSearcherService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackagesUpdatesSearcherService
    {
        #region Methods
        IEnumerable<IPackageDetails> SearchForUpdates(bool? allowPrerelease = null, bool authenticateIfRequired = true);
        #endregion
    }
}