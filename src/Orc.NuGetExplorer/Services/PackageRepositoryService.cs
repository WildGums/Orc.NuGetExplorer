// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageRepositoryService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using NuGet;
    using Path = Catel.IO.Path;

    public class PackageRepositoryService : IPackageRepositoryService
    {
        #region Fields
        private readonly IPackageSourceService _packageSourceService;
        #endregion

        #region Constructors
        public PackageRepositoryService(IPackageSourceService packageSourceService)
        {
            Argument.IsNotNull(() => packageSourceService);

            _packageSourceService = packageSourceService;
        }
        #endregion

        #region Methods
        public IDictionary<string, IPackageRepository> GetRepositories(string categoryName)
        {
            switch (categoryName)
            {
                case RepoCategoryName.Installed:
                    return GetInstalledRepo();
                case RepoCategoryName.Online:
                    return GetOnlineRepos();
            }

            return new Dictionary<string, IPackageRepository>();
        }

        private IDictionary<string, IPackageRepository> GetOnlineRepos()
        {
            var repositoryFactory = PackageRepositoryFactory.Default;

            var packageSources = _packageSourceService.GetPackageSources().ToArray();
            var aggregateRepository = new AggregateRepository(repositoryFactory, packageSources.Select(x => x.Source), true);
            var repositories = new Dictionary<string, IPackageRepository>
            {
                {RepoName.All, aggregateRepository}
            };

            foreach (var packageSource in packageSources)
            {
                var repo = repositoryFactory.CreateRepository(packageSource.Source);
                repositories.Add(packageSource.Name, repo);
            }

            return repositories;
        }

        private static IDictionary<string, IPackageRepository> GetInstalledRepo()
        {
            var applicationDataDirectory = Path.GetApplicationDataDirectory();
            var path = Path.Combine(applicationDataDirectory, "plugins");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            return new Dictionary<string, IPackageRepository>
            {
                {RepoName.All, new LocalPackageRepository(path, true)}
            };
        }
        #endregion
    }
}