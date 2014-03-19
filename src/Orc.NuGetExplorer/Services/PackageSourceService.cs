// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetFeedService.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.ObjectModel;
    using Catel;
    using NuGet;

    public class PackageSourceService : IPackageSourceService
    {
        public PackageSourceService()
        {
            PackageSources = new ObservableCollection<PackageSource>();
            PackageSources.Add(new PackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true));
        }

        #region Methods
        public IPackageRepository GetPackageRepository(string packageSource)
        {
            Argument.IsNotNullOrWhitespace(() => packageSource);

            return PackageRepositoryFactory.Default.CreateRepository(packageSource);
        }

        public ObservableCollection<PackageSource> PackageSources { get; private set; } 
        #endregion

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
    }
}