namespace Orc.NuGetExplorer.Packaging;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Data;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

/// <summary>
/// Aggregate package metadata containing preloaded informations about versions
/// </summary>
internal class MultiVersionPackageSearchMetadata : ClonedPackageSearchMetadata, IPackageDetails
{
    private readonly List<string> _availableVersions = new();
    private string? _overridenVersion = null;

    public MultiVersionPackageSearchMetadata(IEnumerable<IPackageSearchMetadata> availableVersions)
    {
        _availableVersions.AddRange(availableVersions.Select(x => x.Identity.Version.ToFullString()));
    }

    public string Id => Identity.Id;

    public string FullName => Identity.ToFullString();

    public Version Version => Identity.Version.Version;

    public NuGetVersion NuGetVersion => Identity.Version;

    //todo
    public bool IsLatestVersion { get; }

    public bool IsPrerelease => Identity?.Version.IsPrerelease ?? false;

    public bool? IsInstalled { get; set; }

    public string SelectedVersion
    {
        get
        {
            return _overridenVersion ?? Identity.Version.ToString(); //selected version by default is version from identity
        }
        set
        {
            _overridenVersion = value;
        }
    }

    public IValidationContext? ValidationContext { get; set; } = new ValidationContext();

    IEnumerable<string> IPackageDetails.Authors => Authors.SplitOrEmpty();

    long? IPackageDetails.DownloadCount => DownloadCount;

    public PackageIdentity GetIdentity()
    {
        return Identity;
    }

    public void ResetValidationContext()
    {
        ValidationContext = new ValidationContext();
    }

    public override string ToString()
    {
        if (Identity is null)
        {
            return "(no identity)";
        }

        return $"{Id} ({SelectedVersion})";
    }
}
