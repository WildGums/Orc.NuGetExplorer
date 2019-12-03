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
        #region Methods
        Task ShowPackagesExplorerAsync();
        Task ShowPackagesExplorerAsync(ExplorerTab openTab, bool searchIncludePrerelease = false);
        Task ShowPackagesSourceSettingsAsync();
        #endregion
    }
}
