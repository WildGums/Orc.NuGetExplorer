// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepoNavigationFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IRepoNavigationFactory
    {
        #region Methods
        ReposNavigator CreateRepoNavigator();
        #endregion
    }
}