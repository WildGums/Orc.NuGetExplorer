// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsWindow.xaml.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer.Example.Views
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