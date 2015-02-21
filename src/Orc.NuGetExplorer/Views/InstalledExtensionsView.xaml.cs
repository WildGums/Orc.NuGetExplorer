// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledExtensionsView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for InstalledExtensionsView.xaml.
    /// </summary>
    internal partial class InstalledExtensionsView
    {
        #region Fields
        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSourceNavigationItem", typeof (string),
            typeof (InstalledExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledExtensionsView"/> class.
        /// </summary>
        public InstalledExtensionsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes static members of the <see cref="InstalledExtensionsView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static InstalledExtensionsView()
        {
            typeof (InstalledExtensionsView).AutoDetectViewPropertiesToSubscribe();
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