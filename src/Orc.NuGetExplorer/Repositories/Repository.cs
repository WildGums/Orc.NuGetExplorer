// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Repository.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class Repository : IRepository
    {
        #region Constructors
        public Repository(IPackageRepository packageRepository)
        {
            NuGetRepository = packageRepository;
        }
        #endregion

        #region Properties
        public string Source
        {
            get { return NuGetRepository.Source; }
        }

        public bool SupportsPrereleasePackages
        {
            get { return NuGetRepository.SupportsPrereleasePackages; }
        }

        public IPackageRepository NuGetRepository { get; private set; }
        #endregion
    }
}