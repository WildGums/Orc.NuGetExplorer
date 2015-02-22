// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet;

    public class PackageRepositoryService : IPackageRepositoryService
    {
        public IPackageRepository GetRepository(string actionName, IEnumerable<PackageSource> packageSources)
        {
            IPackageRepository repo = new AggregateRepository(PackageRepositoryFactory.Default, packageSources.Select(x => x.Source), true);
            return repo;
        }
    }
}