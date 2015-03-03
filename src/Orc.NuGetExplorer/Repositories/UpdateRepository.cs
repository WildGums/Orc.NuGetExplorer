// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateRepository.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer.Repositories
{
    using System.Linq;
    using NuGet;

    public class UpdateRepository : IPackageRepository
    {
        private readonly IPackageRepository _destinationRepository;
        private readonly IPackageRepository _sourceRepository;

        public UpdateRepository(IPackageRepository destinationRepository, IPackageRepository sourceRepository)
        {
            _destinationRepository = destinationRepository;
            _sourceRepository = sourceRepository;
        }

        public bool IncludePrerelease { get; set; }

        public IQueryable<IPackage> GetPackages()
        {
            var packageNames = _destinationRepository.GetPackages().Select(x => new PackageName(x.Id, x.Version));
            return _sourceRepository.GetUpdates(packageNames, IncludePrerelease, true).AsQueryable();
        }

        public void AddPackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public void RemovePackage(IPackage package)
        {
            throw new System.NotImplementedException();
        }

        public string Source
        {
            get { throw new System.NotImplementedException(); }
        }

        public PackageSaveModes PackageSaveMode { get; set; }

        public bool SupportsPrereleasePackages
        {
            get { return _destinationRepository.SupportsPrereleasePackages && _sourceRepository.SupportsPrereleasePackages; }
        }
    }
}