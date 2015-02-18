// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.ObjectModel;
    using Catel;
    using NuGet;

    public class PackageSourceService : IPackageSourceService
    {
        #region Constructors
        public PackageSourceService()
        {
            PackageSources = new ObservableCollection<PackageSource>();
            PackageSources.Add(new PackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true));
        }
        #endregion

        #region Properties
        public ObservableCollection<PackageSource> PackageSources { get; private set; }
        #endregion

        #region Methods
        public IPackageRepository GetPackageRepository(string packageSource)
        {
            Argument.IsNotNullOrWhitespace(() => packageSource);

            return PackageRepositoryFactory.Default.CreateRepository(packageSource);
        }

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}