// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System;
    using System.Windows;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for ExtensionsView.xaml.
    /// </summary>
    public partial class ExtensionsView
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsView"/> class.
        /// </summary>
        public ExtensionsView()
        {
            InitializeComponent();
        }

        static ExtensionsView()
        {
            typeof (ExtensionsView).AutoDetectViewPropertiesToSubscribe();
        }
        #endregion

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public NamedRepo NamedRepository
        {
            get { return (NamedRepo) GetValue(NamedRepositoryProperty); }
            set { SetValue(NamedRepositoryProperty, value); }
        }

        public static readonly DependencyProperty NamedRepositoryProperty = DependencyProperty.Register("NamedRepository", typeof(NamedRepo),
            typeof(ExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));


        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string ActionName
        {
            get { return (string)GetValue(ActionNameProperty); }
            set { SetValue(ActionNameProperty, value); }
        }

        public static readonly DependencyProperty ActionNameProperty = DependencyProperty.Register("ActionName", typeof(string),
            typeof(ExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}