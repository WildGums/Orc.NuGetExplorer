namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    using Catel.Data;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;

    public interface IPackageDetails
    {
        #region Properties
        string Id { get; }

        string FullName { get; }

        string Description { get; }

        Uri IconUrl { get; }

        Version Version { get; }

        NuGetVersion NuGetVersion { get; }

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

        PackageIdentity GetIdentity();

        #endregion
    }
}
