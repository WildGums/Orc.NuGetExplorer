﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using NuGet;

    public interface IPackageSourceService
    {
        #region Methods
        IEnumerable<PackageSource> GetPackageSources();
        void Save();
        void Load();
        #endregion
    }
}