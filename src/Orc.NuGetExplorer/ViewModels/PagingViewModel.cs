// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Catel.MVVM;

    public class PagingViewModel : ViewModelBase
    {
        #region Constructors
        public PagingViewModel()
        {
            PagingItems = new ObservableCollection<PagingItemInfo>();
            GoToPage = new Command<PagingItemInfo>(OnGoToPageExecute, OnGoToPageCanExecute);
        }
        #endregion

        #region Properties
        public ObservableCollection<PagingItemInfo> PagingItems { get; set; }
        public int VisiblePages { get; set; }
        public int ItemsCount { get; set; }
        public int ItemsPerPage { get; set; }
        public int ItemIndex { get; set; }
        #endregion

        #region Methods
        private void MoveForward()
        {
            ItemIndex += ItemsPerPage;
        }

        private void MoveBack()
        {
            ItemIndex -= ItemsPerPage;
        }

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
            PagingItems.Clear();

            var rightItems = ItemsCount - ItemIndex;
            var leftItems = ItemsCount - rightItems;

            var rightPages = Math.Ceiling(rightItems/(double) ItemsPerPage) - 1;
            var leftPages = Math.Ceiling(leftItems / (double)ItemsPerPage);

            if (leftPages > 0)
            {
               // PagingItems.Add(new PagingItemInfo());
            }
        }
        #endregion

        #region Commands
        public Command<PagingItemInfo> GoToPage { get; private set; }

        private void OnGoToPageExecute(PagingItemInfo pagingItem)
        {
        }

        private bool OnGoToPageCanExecute(PagingItemInfo pagingItem)
        {
            return true;
        }
        #endregion
    }
}