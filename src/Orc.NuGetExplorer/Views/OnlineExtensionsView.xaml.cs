// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for OnlineExtensionsView.xaml.
    /// </summary>
    internal partial class OnlineExtensionsView
    {
        #region Fields
        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(PackageSourcesNavigationItem),
            typeof (OnlineExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineExtensionsView"/> class.
        /// </summary>
        public OnlineExtensionsView()
        {
            InitializeComponent();
        }

        static OnlineExtensionsView()
        {
            typeof (OnlineExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public PackageSourcesNavigationItem PackageSource
        {
            get { return (PackageSourcesNavigationItem)GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }
        #endregion
    }
}