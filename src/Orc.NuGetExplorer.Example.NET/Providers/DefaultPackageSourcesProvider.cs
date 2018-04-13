// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPackageSourcesProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;

    public class DefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        #region Fields
        private readonly IPackageSourceFactory _packageSourceFactory;
        #endregion

        #region Constructors
        public DefaultPackageSourcesProvider(IPackageSourceFactory packageSourceFactory)
        {
            Argument.IsNotNull(() => packageSourceFactory);

            _packageSourceFactory = packageSourceFactory;
        }
        #endregion

        #region Methods
        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return Enumerable.Empty<IPackageSource>();
            //yield return _packageSourceFactory.CreatePackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true);
        }
        #endregion
    }
}