// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Catel;
    using NuGet;

    public class PackageSourceService : IPackageSourceService
    {
        #region Constructors
        public PackageSourceService()
        {

        }
        #endregion

        #region Methods
        public IEnumerable<PackageSource> GetPackageSources()
        {
            yield return new PackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true);
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