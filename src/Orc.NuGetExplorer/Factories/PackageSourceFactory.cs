// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    internal class PackageSourceFactory : IPackageSourceFactory
    {
        public IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial)
        {
            return new NuGetPackageSource(source, name, isEnabled, isOfficial);
        }
    }
}