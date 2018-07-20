// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageSourceFactory.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal class PackageSourceFactory : IPackageSourceFactory
    {
        #region Methods
        public IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial)
        {
            return new NuGetPackageSource(source, name, isEnabled, isOfficial);
        }
        #endregion
    }
}