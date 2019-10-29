using System.Collections.Generic;
using NuGet.Configuration;

namespace Orc.NuGetExplorer.Models
{
    public interface INuGetSettings
    {
        IReadOnlyList<PackageSource> GetAllPackageSources();
    }
}
