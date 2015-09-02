// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSource.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        #region Properties
        bool IsEnabled { get; }
        bool IsOfficial { get; }
        string Name { get; }
        string Source { get; }
        #endregion
    }
}