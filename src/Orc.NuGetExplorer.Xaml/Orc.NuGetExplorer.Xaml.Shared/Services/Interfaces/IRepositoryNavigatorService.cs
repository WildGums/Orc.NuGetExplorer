// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryNavigatorService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal interface IRepositoryNavigatorService
    {
        #region Properties
        RepositoryNavigator Navigator { get; }
        #endregion
    }
}