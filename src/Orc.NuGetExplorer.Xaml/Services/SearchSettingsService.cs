// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchSettingsService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
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