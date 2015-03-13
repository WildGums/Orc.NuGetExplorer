// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet;

    internal class NuGetPackageManager : PackageManager, INuGetPackageManager
    {
        #region Constructors
        public NuGetPackageManager(IPackageRepositoryService packageRepositoryService, INuGetConfigurationService nuGetConfigurationService,
            ILogger logger)
            : this(packageRepositoryService.GetAggregateRepository(), nuGetConfigurationService.GetDestinationFolder())
        {
            Argument.IsNotNull(() => logger);

            Logger = logger;
        }

        public NuGetPackageManager(IPackageRepository sourceRepository, string path)
            : base(sourceRepository, path)
        {
        }

        #endregion
    }
}