namespace Orc.NuGetExplorer;

using System;
using NuGet.Packaging.Core;

public class PackageInstallationContext
{
    public required PackageIdentity Package { get; set; }
    public required IExtensibleProject Project { get; set; }
    public Func<PackageIdentity, bool>? PackagePredicate { get; set; }
    public bool AllowMultipleVersions { get; set; }
    public bool IgnoreMissingPackages { get; set; }
    public bool ShowErrors { get; set; }
}
