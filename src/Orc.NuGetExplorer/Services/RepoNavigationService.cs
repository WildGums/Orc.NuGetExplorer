// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepoNavigationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    public class RepoNavigationService : IRepoNavigationService
    {
        #region Fields
        private ReposNavigator _navigator;
        private readonly IRepoNavigationFactory _repoNavigationFactory;
        #endregion

        #region Constructors
        public RepoNavigationService(IRepoNavigationFactory repoNavigationFactory)
        {
            Argument.IsNotNull(() => repoNavigationFactory);

            _repoNavigationFactory = repoNavigationFactory;
        }
        #endregion

        #region Methods
        public ReposNavigator GetNavigator()
        {
            if (_navigator == null)
            {
                _navigator = _repoNavigationFactory.CreateRepoNavigator();
            }

            return _navigator;
        }
        #endregion
    }
}