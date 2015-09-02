// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceFactory.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageSourceFactory
    {
        #region Methods
        IPackageSource CreatePackageSource(string source, string name, bool isEnabled, bool isOfficial);
        #endregion
    }
}