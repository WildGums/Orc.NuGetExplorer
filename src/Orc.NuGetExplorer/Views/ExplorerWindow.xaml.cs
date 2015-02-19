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
    /// Interaction logic for ExplorerView.xaml.
    /// </summary>
    internal partial class ExplorerView
    {
        #region Constructors
        public ExplorerView()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplorerView"/> class.
        /// </summary>
        public ExplorerView(ExplorerViewModel viewModel)
            : base(viewModel, DataWindowMode.Close)
        {
            InitializeComponent();
        }
        #endregion
    }
}