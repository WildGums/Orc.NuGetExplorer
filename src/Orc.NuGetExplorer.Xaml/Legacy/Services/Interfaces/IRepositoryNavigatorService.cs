// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRepositoryNavigatorService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    internal interface IRepositoryNavigatorService
    {
        #region Properties
        RepositoryNavigator Navigator { get; }
        #endregion

        void Initialize();
    }
}