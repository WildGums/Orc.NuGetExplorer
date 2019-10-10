// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchResultService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
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