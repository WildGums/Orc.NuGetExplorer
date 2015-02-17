// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateExtensionsView.xaml.cs" company="Orcomp development team">
//   Copyright (c) 2008 - 2015 Orcomp development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;

    public partial class UpdateExtensionsView
    {
        #region Constructors
        /// <summary>
        /// Initializes static members of the <see cref="UpdateExtensionsView"/> class.
        /// </summary>
        /// <remarks>This method is required for design time support.</remarks>
        static UpdateExtensionsView()
        {
            typeof(UpdateExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExtensionsView"/> class.
        /// </summary>
        public UpdateExtensionsView()
        {
            InitializeComponent();
        }
        #endregion

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string)GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }

        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(string),
            typeof(UpdateExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}