// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageSourceProvider.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using NuGet;

    internal class NuGetPackageSourceProvider : PackageSourceProvider
    {
        #region Constructors
        public NuGetPackageSourceProvider(ISettings settingsManager, IDefaultPackageSourcesProvider defaultPackageSourcesProvider)
            : base(settingsManager, defaultPackageSourcesProvider.GetDefaultPackages().ToPackageSourceInstances())
        {
        }
        #endregion
    }
}