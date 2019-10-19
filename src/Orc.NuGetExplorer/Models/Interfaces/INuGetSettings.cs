using NuGet.Configuration;
using System.Collections.Generic;

namespace Orc.NuGetExplorer.Models
{
    public interface INuGetSettings
    {
        IReadOnlyList<PackageSource> GetAllPackageSources();
    }
}
