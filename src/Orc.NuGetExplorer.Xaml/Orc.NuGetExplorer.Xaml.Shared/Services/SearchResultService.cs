// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal class SearchResultService : ISearchResultService
    {
        #region Constructors
        public SearchResultService()
        {
            SearchResult = new SearchResult();
        }
        #endregion

        #region Properties
        public SearchResult SearchResult { get; private set; }
        #endregion
    }
}