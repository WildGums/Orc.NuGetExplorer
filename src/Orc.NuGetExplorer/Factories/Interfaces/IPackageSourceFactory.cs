// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSourceFactory.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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