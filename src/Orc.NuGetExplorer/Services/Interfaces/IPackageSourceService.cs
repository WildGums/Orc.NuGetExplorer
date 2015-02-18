// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.ObjectModel;
    using NuGet;

    public interface IPackageSourceService
    {
        #region Properties
        ObservableCollection<PackageSource> PackageSources { get; }
        #endregion

        #region Methods
        IPackageRepository GetPackageRepository(string packageSource);
        void Save();
        void Load();
        #endregion
    }
}