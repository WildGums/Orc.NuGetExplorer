// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigationFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;

    internal class RepositoryNavigationFactory : IRepositoryNavigationFactory
    {
        #region Fields
        private readonly IPackageRepositoryService _packageRepositoryService;
        #endregion

        #region Constructors
        public RepositoryNavigationFactory(IPackageRepositoryService packageRepositoryService)
        {
            Argument.IsNotNull(() => packageRepositoryService);

            _packageRepositoryService = packageRepositoryService;
        }
        #endregion

        #region Methods
        public RepositoryNavigator CreateRepoNavigator()
        {
            var navigator = new RepositoryNavigator();

            navigator.RepoCategories.Add(CreateRepoCategory(RepositoryCategoryType.Installed));
            navigator.RepoCategories.Add(CreateRepoCategory(RepositoryCategoryType.Online));
            navigator.RepoCategories.Add(CreateRepoCategory(RepositoryCategoryType.Update));

            return navigator;
        }

        private RepositoryCategory CreateRepoCategory(RepositoryCategoryType category)
        {
            var repoCategory = new RepositoryCategory(category);

            foreach (var repository in _packageRepositoryService.GetRepositories(category))
            {
                repoCategory.Repos.Add(new NamedRepository
                {
                    Name = repository.Key, 
                    Value = repository.Value
                });
            }

            return repoCategory;
        }
        #endregion
    }
}