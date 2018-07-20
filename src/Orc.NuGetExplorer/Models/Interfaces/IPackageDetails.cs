// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageDetails.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    using Catel.Data;

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

        IEnumerable<string> Authors { get; }

        DateTimeOffset? Published { get; }

        int? DownloadCount { get; }

        string Dependencies { get; }

        bool? IsInstalled { get; set; }

        IList<string> AvailableVersions { get; }

        string SelectedVersion { get; set; }

        IValidationContext ValidationContext { get; }

        #endregion

        #region Methods
        void ResetValidationContext();
        #endregion
    }
}