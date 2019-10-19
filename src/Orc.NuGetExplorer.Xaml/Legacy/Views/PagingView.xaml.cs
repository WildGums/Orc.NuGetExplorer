// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingView.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for PagingView.xaml
    /// </summary>
    internal partial class PagingView
    {
        #region Constructors
        static PagingView()
        {
            typeof (PagingView).AutoDetectViewPropertiesToSubscribe();
        }

        public PagingView()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public int VisiblePages
        {
            get { return (int) GetValue(VisiblePagesProperty); }
            set { SetValue(VisiblePagesProperty, value); }
        }

        public static readonly DependencyProperty VisiblePagesProperty = DependencyProperty.Register("VisiblePages", typeof(int),
            typeof(PagingView), new FrameworkPropertyMetadata(5, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public int ItemsCount
        {
            get { return (int) GetValue(ItemsCountProperty); }
            set { SetValue(ItemsCountProperty, value); }
        }

        public static readonly DependencyProperty ItemsCountProperty = DependencyProperty.Register("ItemsCount", typeof(int), typeof(PagingView),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        

        [ViewToViewModel("ItemsPerPage", MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public int ItemsPerPage
        {
            get { return (int) GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        public static readonly DependencyProperty ItemsPerPageProperty = DependencyProperty.Register("ItemsPerPage", typeof(int),
            typeof(PagingView), new FrameworkPropertyMetadata(10, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayDoNothing)]
        public int ItemIndex
        {
            get { return (int) GetValue(ItemIndexProperty); }
            set { SetValue(ItemIndexProperty, value); }
        }

        public static readonly DependencyProperty ItemIndexProperty = DependencyProperty.Register("ItemIndex", typeof(int), typeof(PagingView),
            new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}