// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepoNavigationFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
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
        public IEnumerable<RepoCategory> CreateRepoCategories()
        {
            var repoCategories = new[]
            {
                CreateRepoCategory(RepoCategoryName.Installed),
                CreateRepoCategory(RepoCategoryName.Online),
                CreateRepoCategory(RepoCategoryName.Update)
            };

            repoCategories[0].IsSelected = true;
            return repoCategories;
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