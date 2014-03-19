// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetFeedService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Services
{
    using System.Collections.ObjectModel;
    using Catel;
    using NuGet;

    public class NuGetFeedService : INuGetFeedService
    {
        public NuGetFeedService()
        {
            PackageSources = new ObservableCollection<string>();
        }

        #region Methods
        public IPackageRepository GetPackageRepository(string packageSource)
        {
            Argument.IsNotNullOrWhitespace(() => packageSource);

            return PackageRepositoryFactory.Default.CreateRepository(packageSource);
        }

        public ObservableCollection<string> PackageSources { get; private set; } 
        #endregion
    }
}