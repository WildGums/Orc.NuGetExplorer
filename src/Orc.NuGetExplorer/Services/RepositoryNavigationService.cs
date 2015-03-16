// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using Catel.Logging;

    internal class RepositoryNavigationService : IRepositoryNavigationService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IRepositoryNavigationFactory _repositoryNavigationFactory;
        private RepositoryNavigator _navigator;
        #endregion

        #region Constructors
        public RepositoryNavigationService(IRepositoryNavigationFactory repositoryNavigationFactory)
        {
            Argument.IsNotNull(() => repositoryNavigationFactory);

            _repositoryNavigationFactory = repositoryNavigationFactory;
        }
        #endregion

        #region Methods
        public RepositoryNavigator GetNavigator()
        {
            if (_navigator == null)
            {
                _navigator = _repositoryNavigationFactory.CreateRepoNavigator();
            }

            return _navigator;
        }
        #endregion
    }
}