// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigatorService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    internal class RepositoryNavigatorService : IRepositoryNavigatorService
    {
        #region Fields
        private readonly IRepositoryNavigationFactory _repositoryNavigationFactory;
        private readonly IRepositoryService _repositoryService;
        #endregion

        #region Constructors
        public RepositoryNavigatorService(IRepositoryNavigationFactory repositoryNavigationFactory, IRepositoryService repositoryService)
        {
            Argument.IsNotNull(() => repositoryNavigationFactory);
            Argument.IsNotNull(() => repositoryService);

            _repositoryNavigationFactory = repositoryNavigationFactory;
            _repositoryService = repositoryService;
        }
        #endregion

        #region Properties
        public RepositoryNavigator Navigator { get; private set; }
        #endregion

        #region Methods
        public void Initialize()
        {
            Navigator = _repositoryNavigationFactory.CreateRepoNavigator();
        }
        #endregion
    }
}