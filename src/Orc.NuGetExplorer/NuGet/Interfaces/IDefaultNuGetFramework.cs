namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using NuGet.Frameworks;

public interface IDefaultNuGetFramework
{
    IReadOnlyList<NuGetFramework> GetHighest();
    IReadOnlyList<NuGetFramework> GetLowest();
}
