// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPackageSourcesProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer.Example
{
    using System.Collections.Generic;
    using Catel;

    public class DefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        private readonly IPackageSourceFactory _packageSourceFactory;

        public DefaultPackageSourcesProvider(IPackageSourceFactory packageSourceFactory)
        {
            Argument.IsNotNull(() => packageSourceFactory);

            _packageSourceFactory = packageSourceFactory;
        }

        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            yield return _packageSourceFactory.CreatePackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true);
        }
    }
}