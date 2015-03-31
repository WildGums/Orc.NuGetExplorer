// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RepositoryNavigationView.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Catel.MVVM.Views;

    /// <summary>
    /// Interaction logic for RepositoryNavigationView.xaml
    /// </summary>
    internal partial class RepositoryNavigationView
    {
        #region Constructors
        public RepositoryNavigationView()
        {
            InitializeComponent();
        }
        #endregion

       /* [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public IRepository SelectedRepository
        {
            get { return (IRepository)GetValue(SelectedRepositoryProperty); }
            set { SetValue(SelectedRepositoryProperty, value); }
        }

        public static readonly DependencyProperty SelectedRepositoryProperty = DependencyProperty.Register("SelectedRepository", typeof(IRepository),
            typeof(RepositoryNavigationView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));*/

    }
}