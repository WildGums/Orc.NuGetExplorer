// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Example.ViewModels
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Fody;
    using Catel.MVVM;
    using Models;

    public class MainViewModel : ViewModelBase
    {
        #region Fields
        private readonly IPackagesUIService _packagesUiService;
        #endregion

        #region Constructors
        public MainViewModel(IPackagesUIService packagesUiService, IEchoService echoService)
        {
            Argument.IsNotNull(() => packagesUiService);
            Argument.IsNotNull(() => echoService);

            _packagesUiService = packagesUiService;

            Echo = echoService.GetPackageManagementEcho();

            ShowExplorer = new TaskCommand(OnShowExplorerExecute);
        }
        #endregion

        [Model]
        [Expose("Lines")]
        public PackageManagementEcho Echo { get; private set; }

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
            await _packagesUiService.ShowPackagesExplorer();
        }
        #endregion
    }
}