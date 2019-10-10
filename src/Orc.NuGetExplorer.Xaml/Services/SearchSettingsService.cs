// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchSettingsService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    internal class SearchSettingsService : ISearchSettingsService
    {
        #region Constructors
        public SearchSettingsService()
        {
            SearchSettings = new SearchSettings();
        }
        #endregion

        #region Properties
        public SearchSettings SearchSettings { get; private set; }
        #endregion
    }
}