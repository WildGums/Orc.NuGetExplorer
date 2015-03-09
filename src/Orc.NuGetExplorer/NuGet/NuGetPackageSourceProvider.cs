// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageSourceProvider.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Extensions;
    using NuGet;

    internal class NuGetPackageSourceProvider : PackageSourceProvider
    {
        #region Constructors
        public NuGetPackageSourceProvider(ISettings settingsManager, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : base(settingsManager, defaultPackageSourcesProvider.GetDefaultPackages().ToNuGetPackageSources())
        {
        }
        #endregion
    }
}