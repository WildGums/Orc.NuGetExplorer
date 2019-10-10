// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPagingService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
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