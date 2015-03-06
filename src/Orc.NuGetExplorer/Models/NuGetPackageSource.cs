// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetPackageSource.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using NuGet;

    internal class NuGetPackageSource : PackageSource, IPackageSource
    {
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
    }
}