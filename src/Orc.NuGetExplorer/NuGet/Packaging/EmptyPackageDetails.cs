namespace Orc.NuGetExplorer.Packaging;

using System;
using System.Collections.Generic;
using Catel.Data;
using NuGet.Packaging.Core;
using NuGet.Versioning;

public class EmptyPackageDetails : IPackageDetails
{
    private readonly PackageIdentity _package;

    public EmptyPackageDetails(PackageIdentity package)
    {
        _package = package;

        ResetValidationContext();

        FullName = package.ToFullString();

        Description = string.Empty;
        Authors = Array.Empty<string>();
        Title = string.Empty;
        Dependencies = string.Empty;
        SelectedVersion = string.Empty;
    }

    #region IPackageDetails
    public string Id => _package.Id;

    public string FullName { get; }

    public string Description { get; }

    public Uri? IconUrl { get; }

    public Version Version => NuGetVersion.Version;

    public NuGetVersion NuGetVersion => _package.Version;

    public bool IsLatestVersion => false;

    public bool IsPrerelease => NuGetVersion?.IsPrerelease ?? false;

    public string Title { get; }

    public IEnumerable<string> Authors { get; }

    public DateTimeOffset? Published { get; }

    public long? DownloadCount { get; }

    public string Dependencies { get; }

    public bool? IsInstalled { get; set; }

    public string SelectedVersion { get; set; }

    public IValidationContext? ValidationContext { get; set; }

    public PackageIdentity GetIdentity()
    {
        return _package;
    }

    public void ResetValidationContext()
    {
        ValidationContext = new ValidationContext();
    }

    #endregion
}
