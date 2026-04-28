namespace Orc.NuGetExplorer;

using System.Collections.Generic;

public interface IDefaultPackageSourcesProvider
{
    string DefaultSource { get; set; }

    IReadOnlyList<IPackageSource> GetDefaultPackages();
}
