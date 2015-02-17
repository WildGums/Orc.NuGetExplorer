// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackageQueryService
    {
        IEnumerable<PackageDetails> GetPackages(string packageSource, string filter = null, int skip = 0, int take = 10);

        IEnumerable<PackageDetails> GetPackages(string[] packageSources, string filter = null, int skip = 0, int take = 10);
    }
}