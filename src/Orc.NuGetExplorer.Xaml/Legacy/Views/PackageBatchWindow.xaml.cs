// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageBatchWindow.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Views
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
            : base(viewModel, DataWindowMode.Custom)
        {
            InitializeComponent();
        }
        #endregion
    }
}