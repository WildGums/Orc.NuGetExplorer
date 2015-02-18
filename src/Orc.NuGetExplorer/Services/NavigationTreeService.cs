// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationTreeService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel;

    internal class NavigationTreeService : INavigationTreeService
    {
        private readonly IPackageSourceService _packageSourceService;

        public NavigationTreeService(IPackageSourceService packageSourceService)
        {
            Argument.IsNotNull(() => packageSourceService);

            _packageSourceService = packageSourceService;
        }

        public IEnumerable<NavigationItem> GetNavigationItems()
        {
            return new[] { GetNode("Installed", false), GetNode("Online"), GetNode("Updates") };
        }

        private NavigationItem GetNode(string name, bool addPackageSources = true)
        {
            var navigationItem = new NavigationItem(name);
            navigationItem.Children.Add(new NavigationItem("All"));

            if (!addPackageSources)
            {
                return navigationItem;
            }

            foreach (var packageSource in _packageSourceService.PackageSources)
            {
                navigationItem.Children.Add(new NavigationItem(packageSource.Name));
            }

            return navigationItem;
        }
    }
}