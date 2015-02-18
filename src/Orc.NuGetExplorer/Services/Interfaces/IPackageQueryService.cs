// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageQueryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageQueryService
    {
        #region Methods
        IEnumerable<PackageDetails> GetPackages(string packageSource, string filter = null, int skip = 0, int take = 10);
        IEnumerable<PackageDetails> GetPackages(string[] packageSources, string filter = null, int skip = 0, int take = 10);
        #endregion
    }
}