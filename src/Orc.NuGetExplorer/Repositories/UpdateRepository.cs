// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateRepository.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Linq;
    using Catel;
    using NuGet;

    internal class UpdateRepository : IPackageRepository
    {
        #region Fields
        private readonly IPackageRepository _destinationRepository;
        private readonly IPackageRepository _sourceRepository;
        #endregion

        #region Constructors
        public UpdateRepository(IRepository destinationRepository, IRepository sourceRepository, IRepositoryCacheService repositoryCacheService)
            : this(repositoryCacheService.GetNuGetRepository(destinationRepository), repositoryCacheService.GetNuGetRepository(sourceRepository))
        {
        }

        public UpdateRepository(IPackageRepository destinationRepository, IPackageRepository sourceRepository)
        {
            Argument.IsNotNull(() => destinationRepository);
            Argument.IsNotNull(() => sourceRepository);

            _destinationRepository = destinationRepository;
            _sourceRepository = sourceRepository;
        }
        #endregion

        #region Properties
        public string Source
        {
            get { return string.Empty; }
        }

        public PackageSaveModes PackageSaveMode { get; set; }

        public bool SupportsPrereleasePackages
        {
            get { return _destinationRepository.SupportsPrereleasePackages && _sourceRepository.SupportsPrereleasePackages; }
        }
        #endregion

        #region Methods
        public IQueryable<IPackage> GetPackages()
        {
            var packageNames = _destinationRepository.GetPackages().Select(x => new PackageName(x.Id, x.Version));
            return _sourceRepository.GetUpdates(packageNames, true, false).AsQueryable();
        }

        public void AddPackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
