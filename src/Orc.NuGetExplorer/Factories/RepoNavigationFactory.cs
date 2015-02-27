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
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryName.Installed));
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryName.Online));
            navigator.RepoCategories.Add(CreateRepoCategory(RepoCategoryName.Update));

            return navigator;
        }

        private RepoCategory CreateRepoCategory(string categoryName)
        {
            Argument.IsNotNullOrWhitespace(() => categoryName);

            var repoCategory = new RepoCategory(categoryName);

            foreach (var repository in _packageRepositoryService.GetRepositories(categoryName))
            {
                repoCategory.Repos.Add(new NamedRepo {Name = repository.Key, Value = repository.Value});
            }

            return repoCategory;
        }
        #endregion
    }
}