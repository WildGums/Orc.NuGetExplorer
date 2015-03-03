// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class NuGetPackageManager : PackageManager
    {
        #region Constructors
        public NuGetPackageManager(IPackageRepositoryService packageRepositoryService, INuGetConfigurationService nuGetConfigurationService)
            : this(packageRepositoryService.GetAggregateRepository(), nuGetConfigurationService.GetDestinationFolder())
        {
        }

        public NuGetPackageManager(IPackageRepository sourceRepository, string path) : base(sourceRepository, path)
        {
        }

        public NuGetPackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem) : base(sourceRepository, pathResolver, fileSystem)
        {
        }

        public NuGetPackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem, IPackageRepository localRepository) : base(sourceRepository, pathResolver, fileSystem, localRepository)
        {
        }
        #endregion
    }
}