// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationTreeService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;

    using Catel;

    internal class NavigationTreeService : INavigationTreeService
    {
        private readonly IPackageSourceService _packageSourceService;

        public NavigationTreeService(IPackageSourceService packageSourceService)
        {
            Argument.IsNotNull(() => packageSourceService);

            _packageSourceService = packageSourceService;
        }

        public IEnumerable<NavigationItemsGroup> GetNavigationGroups()
        {
            var navigationItemBases = new[] { GetNavigationItemsGroup("Installed", false), GetNavigationItemsGroup("Online"), GetNavigationItemsGroup("Updates") };
            navigationItemBases[0].IsExpanded = true;
            return navigationItemBases;
        }

        private NavigationItemsGroup GetNavigationItemsGroup(string name, bool addPackageSources = true)
        {
            PackageSourceNavigationItem[] packageSources;
            if (addPackageSources)
            {
                packageSources = _packageSourceService.PackageSources.Select(packageSource => new PackageSourceNavigationItem(packageSource)).ToArray();
            }
            else
            {
                packageSources = new PackageSourceNavigationItem[] { };
            }

            var aggregativeNavigationItem = new AggregativeNavigationItem("All", packageSources);
            var navigationItem = new NavigationItemsGroup(name, aggregativeNavigationItem);
            return navigationItem;
        }
    }
}