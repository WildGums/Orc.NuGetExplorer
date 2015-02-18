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
    public partial class OnlineExtensionsView
    {
        #region Fields
        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof (string),
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

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineExtensionsView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static OnlineExtensionsView()
        {
            typeof (InstalledExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string) GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }
        #endregion
    }
}