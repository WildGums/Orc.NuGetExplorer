// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationItem.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Catel.Data;

    public class NavigationItem : ModelBase
    {
        #region Constructors
        public NavigationItem(string name)
        {
            Name = name;
            Children = new List<NavigationItem>();
        }
        #endregion

        #region Properties
        public NavigationItem Parent { get; set; }
        public IList<NavigationItem> Children { get; private set; }
        public string Name { get; private set; }
        #endregion
    }
}