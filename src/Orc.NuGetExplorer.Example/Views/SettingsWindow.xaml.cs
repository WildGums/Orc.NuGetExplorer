// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsWindow.xaml.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.Views
{
    using Catel.Windows;
    using ViewModels;

    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        #region Constructors
        public SettingsWindow()
            : this(null)
        {
        }

        public SettingsWindow(SettingsViewModel viewModel)
            : base(viewModel, DataWindowMode.OkCancelApply)
        {
            InitializeComponent();
        }
        #endregion
    }
}