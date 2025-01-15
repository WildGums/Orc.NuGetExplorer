namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Threading;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;

public class InstallationContext
{
    public required PackageIdentity Package { get; set; }
    public required IExtensibleProject Project { get; set; }
    public required IReadOnlyList<SourceRepository> Repositories { get; set; }
    public bool IgnoreMissingPackages { get; set; }
    public Func<PackageIdentity, bool>? PackagePredicate { get; set; }
    public bool AllowMultipleVersions { get; set; } = false;
}
