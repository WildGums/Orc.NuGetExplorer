// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDefaultPackageSourcesProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System.Collections.Generic;

    public interface IDefaultPackageSourcesProvider
    {
        #region Methods
        IEnumerable<IPackageSource> GetDefaultPackages();
        #endregion
    }
}