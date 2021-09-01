namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Catel.Windows;

    public interface IPackagesUIService
    {
        #region Properties
        string SettingsTitle { get; set; }
        #endregion

        #region Methods
        Task ShowPackagesExplorerAsync();
        Task ShowPackagesExplorerAsync(INuGetExplorerInitialState initialState);
        Task<bool?> ShowPackagesSourceSettingsAsync();
        IEnumerable<DataWindow> GetOpenedPackageExplorerWindows();
        IEnumerable<DataWindow> GetOpenedPackageSourceWindows();
        #endregion
    }
}
