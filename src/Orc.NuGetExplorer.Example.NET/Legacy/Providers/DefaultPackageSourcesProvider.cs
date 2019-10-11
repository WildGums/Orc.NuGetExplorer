// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPackageSourcesProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Example
{
    using System.Collections.Generic;
    using System.Linq;

    public class DefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        #region Methods
        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return Enumerable.Empty<IPackageSource>();
        }
        #endregion
    }
}
