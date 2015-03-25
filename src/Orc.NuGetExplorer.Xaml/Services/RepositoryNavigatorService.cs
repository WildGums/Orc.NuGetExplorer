﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigatorService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    internal class RepositoryNavigatorService : IRepositoryNavigatorService
    {
        #region Constructors
        public RepositoryNavigatorService(IRepositoryNavigationFactory repositoryNavigationFactory)
        {
            Argument.IsNotNull(() => repositoryNavigationFactory);

            Navigator = repositoryNavigationFactory.CreateRepoNavigator();
        }
        #endregion

        #region Properties
        public RepositoryNavigator Navigator { get; private set; }
        #endregion
    }
}