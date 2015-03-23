// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetails.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    public interface IPackageDetails
    {
        #region Properties
        string Id { get; }
        string FullName { get; }
        string Description { get; }
        Uri IconUrl { get; }
        Version Version { get; }
        string SpecialVersion { get; }
        bool IsAbsoluteLatestVersion { get; }
        bool IsLatestVersion { get; }
        bool IsPrerelease { get; }
        string Title { get; }
        #endregion
    }
}