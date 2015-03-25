// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplorerViewModel.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.ViewModels
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using Catel;
    using Catel.Fody;
    using Catel.Logging;
    using Catel.MVVM;

    internal class ExplorerViewModel : ViewModelBase
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        public ExplorerViewModel(IRepositoryNavigatorService repositoryNavigatorService)
        {
            Argument.IsNotNull(() => repositoryNavigatorService);

            Navigator = repositoryNavigatorService.Navigator;
          
            AccentColorHelper.CreateAccentColorResourceDictionary();
        }
        #endregion

        #region Properties
        [Model]
        [Expose("RepoCategories")]
        [Expose("SelectedRepository")]
        public RepositoryNavigator Navigator { get; private set; }
        #endregion
    }
}