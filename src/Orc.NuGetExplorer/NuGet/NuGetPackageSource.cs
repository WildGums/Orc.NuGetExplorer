// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageSource.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class NuGetPackageSource : PackageSource, IPackageSource
    {
        #region Constructors
        public NuGetPackageSource(string source) : base(source)
        {
        }

        public NuGetPackageSource(string source, string name) : base(source, name)
        {
        }

        public NuGetPackageSource(string source, string name, bool isEnabled) : base(source, name, isEnabled)
        {
        }

        public NuGetPackageSource(string source, string name, bool isEnabled, bool isOfficial) : base(source, name, isEnabled, isOfficial)
        {
        }
        #endregion
    }
}