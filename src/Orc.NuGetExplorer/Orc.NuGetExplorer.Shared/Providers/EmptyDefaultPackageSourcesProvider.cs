// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyDefaultPackageSourcesProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;

    internal class EmptyDefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        #region Methods
        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return Enumerable.Empty<IPackageSource>();
        }
        #endregion
    }
}