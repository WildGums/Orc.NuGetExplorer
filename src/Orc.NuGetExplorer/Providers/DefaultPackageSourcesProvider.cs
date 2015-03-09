// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultPackageSourcesProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    internal class DefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            yield return new NuGetPackageSource("http://www.nuget.org/api/v2/", "NuGet", true, true);
        }
    }
}