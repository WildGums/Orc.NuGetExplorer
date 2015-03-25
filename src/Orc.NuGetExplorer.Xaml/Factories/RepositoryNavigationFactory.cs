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

            navigator.RepoCategories.Add(CreateRepositoryCategory(PackageOperationType.Uninstall));
            navigator.RepoCategories.Add(CreateRepositoryCategory(PackageOperationType.Install));
            navigator.RepoCategories.Add(CreateRepositoryCategory(PackageOperationType.Update));

            return navigator;
        }

        private RepositoryCategory CreateRepositoryCategory(PackageOperationType packageOperationType)
        {
            var repoCategory = new RepositoryCategory(packageOperationType);

            foreach (var repository in _packageRepositoryService.GetRepositories(packageOperationType))
            {
                repoCategory.Repositories.Add(repository);
            }

            return repoCategory;
        }
        #endregion
    }
}