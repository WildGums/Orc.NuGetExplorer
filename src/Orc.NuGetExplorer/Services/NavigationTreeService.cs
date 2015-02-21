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
        #region Fields
        private readonly IPackageSourceService _packageSourceService;
        #endregion

        #region Constructors
        public NavigationTreeService(IPackageSourceService packageSourceService)
        {
            Argument.IsNotNull(() => packageSourceService);

            _packageSourceService = packageSourceService;
        }
        #endregion

        #region Methods
        public IEnumerable<NavigationItemsGroup> CreateNavigationGroups()
        {
            var navigationItemBases = new[] {CreateNavigationItemsGroup("Installed", false), CreateNavigationItemsGroup("Online"), CreateNavigationItemsGroup("Updates")};
            navigationItemBases[0].IsSelected = true;
            return navigationItemBases;
        }

        private NavigationItemsGroup CreateNavigationItemsGroup(string name, bool hasdPackageSources = true)
        {
            Argument.IsNotNullOrWhitespace(() => name);
            if (!hasdPackageSources)
            {
                return new NavigationItemsGroup(name, new PackageSourcesNavigationItem());
            }

            var packageSourceses = _packageSourceService.PackageSources.ToArray();

            var packageSourcesNavigationItems = new List<PackageSourcesNavigationItem>();
            var aggregativeNavigationItem = new PackageSourcesNavigationItem(packageSourceses);
            packageSourcesNavigationItems.Add(aggregativeNavigationItem);

            packageSourcesNavigationItems.AddRange(packageSourceses.Select(packageSource => new PackageSourcesNavigationItem(packageSource)));
            return new NavigationItemsGroup(name, packageSourcesNavigationItems.ToArray());
        }
        #endregion
    }
}