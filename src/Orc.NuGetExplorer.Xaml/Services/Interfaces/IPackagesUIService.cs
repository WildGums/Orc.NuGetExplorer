namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;

    public interface IPackagesUIService
    {
        string SettingsTitle { get; set; }

        Task ShowPackagesExplorerAsync();
        Task ShowPackagesExplorerAsync(INuGetExplorerInitialState initialState);
        Task<bool?> ShowPackagesSourceSettingsAsync();
    }
}
