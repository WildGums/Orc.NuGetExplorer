// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OnlineExtensionsView.xaml.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for OnlineExtensionsView.xaml.
    /// </summary>
    public partial class OnlineExtensionsView : UserControl
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineExtensionsView"/> class.
        /// </summary>
        public OnlineExtensionsView()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string) GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }

        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(string),
            typeof(OnlineExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}