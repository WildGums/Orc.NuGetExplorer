// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstalledExtensionsView.xaml.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for InstalledExtensionsView.xaml.
    /// </summary>
    public partial class InstalledExtensionsView
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="InstalledExtensionsView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static InstalledExtensionsView()
        {
            typeof(InstalledExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledExtensionsView"/> class.
        /// </summary>
        public InstalledExtensionsView()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string)GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }

        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(string),
            typeof(InstalledExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}