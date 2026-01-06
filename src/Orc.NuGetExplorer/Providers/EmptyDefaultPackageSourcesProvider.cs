namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;

public class EmptyDefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
{
    public EmptyDefaultPackageSourcesProvider()
    {
        DefaultSource = string.Empty;
    }

    public string DefaultSource { get; set; }

    public IReadOnlyList<IPackageSource> GetDefaultPackages()
    {
        return Array.Empty<IPackageSource>();
    }
}
