// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExtensionsView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    internal partial class UpdateExtensionsView
    {
        #region Fields
        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("SelectedRepo", typeof (string),
            typeof (UpdateExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExtensionsView"/> class.
        /// </summary>
        public UpdateExtensionsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initializes static members of the <see cref="UpdateExtensionsView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static UpdateExtensionsView()
        {
            typeof (UpdateExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public NamedRepo PackageSource
        {
            get { return (NamedRepo) GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }
        #endregion
    }
}