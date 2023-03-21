namespace Orc.NuGetExplorer;

using System.Threading.Tasks;
using Catel.Services;

public interface IPackagesUIService
{
    string SettingsTitle { get; set; }

    Task ShowPackagesExplorerAsync();
    Task ShowPackagesExplorerAsync(INuGetExplorerInitialState initialState);
    Task<UIVisualizerResult?> ShowPackagesSourceSettingsAsync();
}