// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.ViewModels
{
    using Catel;
    using Catel.MVVM;
    using Catel.Services;
    using Orc.NuGetExplorer.ViewModels;

    public class MainViewModel : ViewModelBase
    {
        private readonly IUIVisualizerService _uiVisualizerService;

        #region Constructors
        public MainViewModel(IUIVisualizerService uiVisualizerService)
        {
            Argument.IsNotNull(() => uiVisualizerService);

            _uiVisualizerService = uiVisualizerService;

            ShowExplorer = new Command(OnShowExplorerExecute);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the ShowExplorer command.
        /// </summary>
        public Command ShowExplorer { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Method to invoke when the ShowExplorer command is executed.
        /// </summary>
        private void OnShowExplorerExecute()
        {
            _uiVisualizerService.ShowDialog<ExplorerViewModel>();
        }
        #endregion
    }
}