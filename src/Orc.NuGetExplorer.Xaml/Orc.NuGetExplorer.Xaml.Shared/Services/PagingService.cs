// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Globalization;
    using Catel;

    internal class PagingService : IPagingService
    {
        #region Methods
        public void UpdatePagingItems(Pager pager)
        {
            Argument.IsNotNull(() => pager);

            if (pager.ItemsPerPage == 0)
            {
                return;
            }

            int leftPages;
            int rightPages;

            var currentPage = GetPagesCount(pager, out leftPages, out rightPages);

            FillLeftPages(pager, leftPages, currentPage);

            FillRightPages(pager, rightPages, currentPage);
        }

        public void Step(Pager pager, int stepValue)
        {
            Argument.IsNotNull(() => pager);

            var newIndex = pager.ItemIndex + pager.ItemsPerPage*stepValue;
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            pager.ItemIndex = newIndex;
        }

        public void MoveToLast(Pager pager)
        {
            Argument.IsNotNull(() => pager);

            pager.ItemIndex = GetLastPageIndex(pager);
            ;
        }

        public bool IsLastPage(Pager pager)
        {
            var lastPageIndex = GetLastPageIndex(pager);
            return pager.ItemIndex >= lastPageIndex;
        }

        public void MoveToFirst(Pager pager)
        {
            Argument.IsNotNull(() => pager);

            if (pager.ItemIndex == 0)
            {
                return;
            }

            pager.ItemIndex = 0;
        }

        public void StepTo(Pager pager, PagingItemInfo pagingItem)
        {
            Argument.IsNotNull(() => pager);

            var stepValue = pagingItem.StepValue;
            Step(pager, stepValue);
        }

        private static int GetLastPageIndex(Pager pager)
        {
            var pagesCount = (int) Math.Ceiling(pager.ItemsCount/(double) pager.ItemsPerPage);
            var i = (pagesCount - 1)*pager.ItemsPerPage;
            return i;
        }

        private int GetPagesCount(Pager pager, out int leftPages, out int rightPages)
        {
            Argument.IsNotNull(() => pager);

            leftPages = 0;
            rightPages = 0;

            var rightItems = pager.ItemsCount - pager.ItemIndex;
            var leftItems = pager.ItemsCount - rightItems;

            var totalRightPages = (int) Math.Ceiling(rightItems/(double) pager.ItemsPerPage) - 1;
            var totalLeftPages = (int) Math.Ceiling(leftItems/(double) pager.ItemsPerPage);

            var totalPagesCount = totalRightPages + totalLeftPages;
            var currentPage = totalLeftPages + 1;
            pager.CurrentPage = currentPage.ToString(CultureInfo.InvariantCulture);

            var leftAdding = true;
            var pageCouner = 0;
            while (pageCouner < pager.VisiblePages - 1 && pageCouner < totalPagesCount)
            {
                if (totalLeftPages.Equals(0))
                {
                    leftAdding = false;
                }

                if (totalRightPages.Equals(0))
                {
                    leftAdding = true;
                }

                if (totalLeftPages > 0 && leftAdding)
                {
                    leftAdding = false;
                    leftPages++;
                    totalLeftPages--;
                    pageCouner++;
                    continue;
                }

                if (totalRightPages > 0 && !leftAdding)
                {
                    leftAdding = true;
                    rightPages++;
                    totalRightPages--;
                    pageCouner++;
                }
            }
            return currentPage;
        }

        private void FillRightPages(Pager pager, int rightPages, int currentPage)
        {
            Argument.IsNotNull(() => pager);

            pager.RightPages.Clear();

            for (var i = 1; i <= rightPages; i++)
            {
                pager.RightPages.Add(new PagingItemInfo((currentPage + i).ToString(CultureInfo.InvariantCulture), i));
            }
        }

        private void FillLeftPages(Pager pager, int leftPagesCount, int currentPage)
        {
            Argument.IsNotNull(() => pager);

            pager.LeftPages.Clear();

            for (var i = leftPagesCount; i > 0; i--)
            {
                pager.LeftPages.Add(new PagingItemInfo((currentPage - i).ToString(CultureInfo.InvariantCulture), -1*i));
            }
        }
        #endregion
    }
}