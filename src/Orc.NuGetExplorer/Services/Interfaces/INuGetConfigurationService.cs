// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetConfigurationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface INuGetConfigurationService
    {
        #region Methods
        string GetDestinationFolder();
        void SetDestinationFolder(string value);
        IEnumerable<IPackageSource> LoadPackageSources();
        void SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true);
        void DeletePackageSource(string name, string source);
        #endregion
    }
}