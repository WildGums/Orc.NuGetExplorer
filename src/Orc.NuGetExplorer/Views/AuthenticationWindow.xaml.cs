// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AuthenticationWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for AuthenticationWindow.xaml
    /// </summary>
    public partial class AuthenticationWindow
    {
        #region Constructors
        public AuthenticationWindow()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationWindow"/> class.
        /// </summary>
        public AuthenticationWindow(AuthenticationViewModel viewModel)
            : base(viewModel, DataWindowMode.OkCancel)
        {
            InitializeComponent();
        }
        #endregion
    }
}