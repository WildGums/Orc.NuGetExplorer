// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml.
    /// </summary>
    internal partial class ExplorerWindow
    {
        #region Constructors
        public ExplorerWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerWindow"/> class.
        /// </summary>
        public ExplorerWindow(ExplorerViewModel viewModel)
            : base(viewModel, DataWindowMode.Custom)
        {
            InitializeComponent();
        }
        #endregion
    }
}