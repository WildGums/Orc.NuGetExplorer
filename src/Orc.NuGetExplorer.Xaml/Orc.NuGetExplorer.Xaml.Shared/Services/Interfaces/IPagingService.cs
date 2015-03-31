// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPagingService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal interface IPagingService
    {
        #region Methods
        void UpdatePagingItems(Pager pager);
        void Step(Pager pager, int stepValue);
        void MoveToLast(Pager pager);
        void MoveToFirst(Pager pager);
        void StepTo(Pager pager, PagingItemInfo pagingItem);
        bool IsLastPage(Pager pager);
        #endregion
    }
}