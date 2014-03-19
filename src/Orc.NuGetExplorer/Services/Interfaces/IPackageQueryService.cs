// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Services
{
    using System.Collections.Generic;
    using Orc.NuGetExplorer.Models;

    public interface IPackageQueryService
    {
        IEnumerable<PackageDetails> GetPackages(string packageSource, string filter = null, int skip = 0, int take = 10);

        IEnumerable<PackageDetails> GetPackages(string[] packageSources, string filter = null, int skip = 0, int take = 10);
    }
}