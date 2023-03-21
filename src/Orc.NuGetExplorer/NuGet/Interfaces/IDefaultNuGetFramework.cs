namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using NuGet.Frameworks;

public interface IDefaultNuGetFramework
{
    IEnumerable<NuGetFramework> GetHighest();
    IEnumerable<NuGetFramework> GetLowest();
}