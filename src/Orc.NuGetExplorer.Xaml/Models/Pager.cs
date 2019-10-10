// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Pager.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System.Collections.ObjectModel;
    using Catel.Data;

    internal class Pager : ModelBase
    {
        #region Constructors
        public Pager()
        {
            LeftPages = new ObservableCollection<PagingItemInfo>();
            RightPages = new ObservableCollection<PagingItemInfo>();
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
    }
}