// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceService.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
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