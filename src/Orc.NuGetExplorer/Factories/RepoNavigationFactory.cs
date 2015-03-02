// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepoNavigationFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    internal class RepoNavigationFactory : IRepoNavigationFactory
    {
        #region Fields
        private readonly IPackageRepositoryService _packageRepositoryService;
        #endregion

        #region Constructors
        public RepoNavigationFactory(IPackageRepositoryService packageRepositoryService)
        {
            Argument.IsNotNull(() => packageRepositoryService);

            _packageRepositoryService = packageRepositoryService;
        }
        #endregion

        #region Methods
        public ReposNavigator CreateRepoNavigator()
        {
            var navigator = new ReposNavigator();
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryType.Installed));
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryType.Online));
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryType.Update));

            return navigator;
        }

        private RepoCategory CreateRepoCategory(RepoCategoryType category)
        {
            var repoCategory = new RepoCategory(category);

            foreach (var repository in _packageRepositoryService.GetRepositories(category))
            {
                repoCategory.Repos.Add(new NamedRepo {Name = repository.Key, Value = repository.Value});
            }

            return repoCategory;
        }
        #endregion
    }
}