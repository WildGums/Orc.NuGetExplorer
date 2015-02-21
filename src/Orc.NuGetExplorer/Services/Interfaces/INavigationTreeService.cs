// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INavigationTreeService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    internal interface INavigationTreeService
    {
        IEnumerable<NavigationItemsGroup> CreateNavigationGroups();
    }
}