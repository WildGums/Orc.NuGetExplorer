// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.MVVM;

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackagesManager _packagesManager;
        #endregion

        #region Constructors
        public MainViewModel(IPackagesManager packagesManager)
        {
            Argument.IsNotNull(() => packagesManager);

            _packagesManager = packagesManager;

            ShowExplorer = new TaskCommand(OnShowExplorerExecute);
        }
        #endregion

        #region Commands
        /// <summary>
        /// Gets the ShowExplorer command.
        /// </summary>
        public TaskCommand ShowExplorer { get; private set; }

        /// <summary>
        /// Method to invoke when the ShowExplorer command is executed.
        /// </summary>
        private async Task OnShowExplorerExecute()
        {
            await _packagesManager.Show();
        }
        #endregion
    }
}