namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using NuGet.Configuration;

public interface INuGetSettings
{
    IReadOnlyList<PackageSource> GetAllPackageSources();
}