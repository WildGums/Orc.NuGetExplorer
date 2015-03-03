// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomPackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class CustomPackageManager : PackageManager
    {
        #region Constructors
        public CustomPackageManager(IPackageRepositoryService packageRepositoryService, INuGetConfigurationService nuGetConfigurationService)
            : this(packageRepositoryService.GetAggregateRepository(), nuGetConfigurationService.GetDestinationFolder())
        {
        }

        public CustomPackageManager(IPackageRepository sourceRepository, string path) : base(sourceRepository, path)
        {
        }

        public CustomPackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem) : base(sourceRepository, pathResolver, fileSystem)
        {
        }

        public CustomPackageManager(IPackageRepository sourceRepository, IPackagePathResolver pathResolver, IFileSystem fileSystem, IPackageRepository localRepository) : base(sourceRepository, pathResolver, fileSystem, localRepository)
        {
        }
        #endregion
    }
}