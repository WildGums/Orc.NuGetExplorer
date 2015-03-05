// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using Catel.MVVM;

    public class PagingViewModel : ViewModelBase
    {
        #region Constructors
        public PagingViewModel()
        {
            LeftPages = new ObservableCollection<PagingItemInfo>();
            RightPages = new ObservableCollection<PagingItemInfo>();
            MoveToPage = new Command<PagingItemInfo>(OnMoveToPageExecute, OnMoveToPageCanExecute);

            MoveToFirst = new Command(OnMoveToFirstExecute, OnMoveToFirstCanExecute);
            MoveBack = new Command(OnMoveBackExecute, OnMoveBackCanExecute);
            MoveForward = new Command(OnMoveForwardExecute, OnMoveForwardCanExecute);
            MoveToLast = new Command(OnMoveToLastExcute, OnMoveToLastCanExecute);
        }
        #endregion

        #region Properties
        public ObservableCollection<PagingItemInfo> LeftPages { get; set; }
        public ObservableCollection<PagingItemInfo> RightPages { get; set; }
        public int VisiblePages { get; set; }
        public int ItemsCount { get; set; }
        public int ItemsPerPage { get; set; }
        public int ItemIndex { get; set; }
        public string CurrentPage { get; set; }
        #endregion

        #region Commands
        public Command MoveToLast { get; private set; }

        private void OnMoveToLastExcute()
        {
            //    var pagesCount = Math.Ceiling(ItemsCount/(double) ItemsPerPage);

        }

        private bool OnMoveToLastCanExecute()
        {
            return true;
        }

        public Command MoveForward { get; private set; }

        private void OnMoveForwardExecute()
        {
            Step(1);
        }

        private bool OnMoveForwardCanExecute()
        {
            return true;
        }

        public Command MoveBack { get; private set; }

        private void OnMoveBackExecute()
        {
            if (ItemIndex == 0)
            {
                return;
            }

            Step(-1);
        }

        private bool OnMoveBackCanExecute()
        {
            return ItemIndex > 0;
        }

        public Command MoveToFirst { get; private set; }

        public void OnMoveToFirstExecute()
        {
            if (ItemIndex == 0)
            {
                return;
            }

            ItemIndex = 0;
        }

        private bool OnMoveToFirstCanExecute()
        {
            return ItemIndex > 0;
        }

        public Command<PagingItemInfo> MoveToPage { get; private set; }

        private void OnMoveToPageExecute(PagingItemInfo pagingItem)
        {
            var stepValue = pagingItem.StepValue;
            Step(stepValue);
        }

        private void Step(int stepValue)
        {
            var newIndex = ItemIndex + ItemsPerPage * stepValue;
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            ItemIndex = newIndex;
        }

        private bool OnMoveToPageCanExecute(PagingItemInfo pagingItem)
        {
            return true;
        }
        #endregion

        #region Methods
        private void OnVisiblePagesChanged()
        {
            UpdatePagingItems();
        }

        private void OnItemsCountChanged()
        {
            UpdatePagingItems();
        }

        private void OnItemsPerPageChanged()
        {
            UpdatePagingItems();
        }

        private void OnItemIndexChanged()
        {
            UpdatePagingItems();
        }

        private void UpdatePagingItems()
        {
            if (ItemsPerPage == 0)
            {
                return;
            }

            int leftPages;
            int rightPages;

            var currentPage = GetPagesCount(out leftPages, out rightPages);

            FillLeftPages(leftPages, currentPage);

            FillRightPages(rightPages, currentPage);
        }

        private int GetPagesCount(out int leftPages, out int rightPages)
        {
            leftPages = 0;
            rightPages = 0;

            var rightItems = ItemsCount - ItemIndex;
            var leftItems = ItemsCount - rightItems;

            var totalRightPages = (int) Math.Ceiling(rightItems/(double) ItemsPerPage) - 1;
            var totalLeftPages = (int) Math.Ceiling(leftItems/(double) ItemsPerPage);

            var totalPagesCount = totalRightPages + totalLeftPages;
            var currentPage = totalLeftPages + 1;
            CurrentPage = currentPage.ToString(CultureInfo.InvariantCulture);

            var leftAdding = true;
            var pageCouner = 0;
            while (pageCouner < VisiblePages - 1 && pageCouner < totalPagesCount)
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

        private void FillRightPages(int rightPages, int currentPage)
        {
            RightPages.Clear();

            for (var i = 1; i <= rightPages; i++)
            {
                RightPages.Add(new PagingItemInfo((currentPage + i).ToString(CultureInfo.InvariantCulture), i));
            }
        }

        private void FillLeftPages(int leftPagesCount, int currentPage)
        {
            LeftPages.Clear();

            for (var i = leftPagesCount; i > 0; i--)
            {
                LeftPages.Add(new PagingItemInfo((currentPage - i).ToString(CultureInfo.InvariantCulture), -1*i));
            }
        }
        #endregion
    }
}