namespace Orc.NuGetExplorer.Views
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using Catel.MVVM.Views;
    using Models;
    using Orc.NuGetExplorer.Controls.Helpers;

    /// <summary>
    /// Interaction logic for ExplorerPageView.xaml
    /// </summary>
    internal partial class ExplorerPageView : Catel.Windows.Controls.UserControl
    {
        private const string ArrowUpResourceKey = "ArrowUpBadgeContent";
        private const string ArrowDownResourceKey = "ArrowDownBadgeContent";
        private const int IndicatorOffset = 2;

        private readonly FrameworkElement _arrowUpResource;
        private readonly FrameworkElement _arrowDownResource;

        private ScrollViewer _infinityboxScrollViewer;
        private bool _isViewportWidthListened = false;

        static ExplorerPageView()
        {
            typeof(ExplorerPageView).AutoDetectViewPropertiesToSubscribe();
        }

        public ExplorerPageView()
        {
            InitializeComponent();

            _arrowUpResource = FindResource(ArrowUpResourceKey) as FrameworkElement;
            _arrowDownResource = FindResource(ArrowDownResourceKey) as FrameworkElement;
        }

        [ViewToViewModel(viewModelPropertyName: "SelectedPackageItem", MappingType = ViewToViewModelMappingType.TwoWayViewModelWins)]
        public NuGetPackage SelectedItemOnPage
        {
            get { return (NuGetPackage)GetValue(SelectedItemOnPageProperty); }
            set { SetValue(SelectedItemOnPageProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="SelectedItemOnPage"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemOnPageProperty =
            DependencyProperty.Register(nameof(SelectedItemOnPage), typeof(NuGetPackage), typeof(ExplorerPageView), new PropertyMetadata(null));

        private void Border_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //fix loading indicator part size
            _infinityboxScrollViewer = _infinityboxScrollViewer ?? WpfHelper.FindVisualChild<ScrollViewer>(infinitybox);

            if (_infinityboxScrollViewer == null)
            {
                return;
            }

            if (!_isViewportWidthListened)
            {
                SubscribeToScrollViewerPropertyChanges();
            }
        }

        private void SubscribeToScrollViewerPropertyChanges()
        {
            //listen ViewportWidth
            if (_isViewportWidthListened)
            {
                return;
            }

            DependencyPropertyDescriptor
                .FromProperty(ScrollViewer.ViewportWidthProperty, typeof(ScrollViewer))
                .AddValueChanged(_infinityboxScrollViewer, (s, e) => OnInfinityScrollViewPortChanged(s, e));

            //manual recount
            OnInfinityScrollViewPortChanged(this, EventArgs.Empty);

            _isViewportWidthListened = true;
        }


        private void OnInfinityScrollViewPortChanged(object sender, EventArgs e)
        {
            indicatorScreen.SetCurrentValue(WidthProperty, _infinityboxScrollViewer.ViewportWidth + IndicatorOffset);
        }

        private void UnsubscribeFromScrollViewerProperyChanged()
        {
            if (_isViewportWidthListened && _infinityboxScrollViewer != null)
            {
                DependencyPropertyDescriptor
                   .FromProperty(ScrollViewer.ViewportWidthProperty, typeof(ScrollViewer))
                   .RemoveValueChanged(_infinityboxScrollViewer, (s, e) => OnInfinityScrollViewPortChanged(s, e));
                _isViewportWidthListened = false;
            }
        }

        protected override void OnUnloaded(EventArgs e)
        {
            UnsubscribeFromScrollViewerProperyChanged();
        }
    }
}
