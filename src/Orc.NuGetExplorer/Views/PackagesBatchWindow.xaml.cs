// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackagesBatchWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for PackagesBatchWindow.xaml
    /// </summary>
    internal partial class PackagesBatchWindow
    {
        #region Constructors
        public PackagesBatchWindow()
            : this(null)
        {
        }

        public PackagesBatchWindow(PackagesBatchViewModel viewModel)
            : base(viewModel, DataWindowMode.Close)
        {
            InitializeComponent();
        }
        #endregion
    }
}