// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using NuGet;

    internal static class IPackageSourceExtensions
    {
        #region Methods
        public static IEnumerable<PackageSource> ToNuGetPackageSources(this IEnumerable<IPackageSource> packageSources)
        {
            Argument.IsNotNull(() => packageSources);

            return packageSources.Select(x => new PackageSource(x.Source, x.Name, x.IsEnabled, x.IsOfficial));
        }
        #endregion
    }
}