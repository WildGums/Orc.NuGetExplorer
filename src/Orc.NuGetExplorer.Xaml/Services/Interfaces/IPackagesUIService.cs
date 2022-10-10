namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;

    public interface IPackagesUIService
    {
        #region Properties
        string SettingsTitle { get; set; }
        #endregion

        #region Methods
        Task ShowPackagesExplorerAsync();
        Task ShowPackagesExplorerAsync(INuGetExplorerInitialState initialState);
        Task<bool?> ShowPackagesSourceSettingsAsync();
        #endregion
    }
}
