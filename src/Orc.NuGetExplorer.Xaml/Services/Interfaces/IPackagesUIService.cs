// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUIService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


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
