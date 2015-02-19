// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NavigationItemBase.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Data;

    internal abstract class NavigationItemBase : ModelBase
    {
        #region Constructors
        public NavigationItemBase(string name)
        {
            Name = name;
        }
        #endregion

        #region Properties
        public string Name { get; private set; }
        #endregion
    }
}