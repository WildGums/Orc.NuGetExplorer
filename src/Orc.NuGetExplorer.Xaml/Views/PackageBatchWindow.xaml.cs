// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageBatchWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for PackageBatchWindow.xaml
    /// </summary>
    internal partial class PackageBatchWindow
    {
        #region Constructors
        public PackageBatchWindow()
            : this(null)
        {
        }

        public PackageBatchWindow(PackageBatchViewModel viewModel)
            : base(viewModel, DataWindowMode.Close)
        {
            InitializeComponent();
        }
        #endregion
    }
}