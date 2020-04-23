namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for ExplorerWindow.xaml
    /// </summary>
    internal partial class ExplorerWindow : DataWindow
    {
        static ExplorerWindow()
        {
            typeof(ExplorerWindow).AutoDetectViewPropertiesToSubscribe();
        }

        public ExplorerWindow() : this(null)
        {
        }

        public ExplorerWindow(ExplorerViewModel viewModel)
            : base(viewModel, DataWindowMode.Custom)
        {
            InitializeComponent();
            ShowInTaskbar = false;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var screenHeight = SystemParameters.PrimaryScreenHeight;
            TopGrid.Height = screenHeight * 2 / 3;

            Title = viewModel.Title;
        }

        #region DependencyProperty

        public static readonly DependencyProperty StartPageProperty =
            DependencyProperty.Register("StartPage", typeof(string), typeof(ExplorerWindow), new PropertyMetadata("Browse", (s, e) => ((ExplorerWindow)s).OnStartPageChanged(s, e)));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewModelToView)]
        public string StartPage
        {
            get { return (string)GetValue(StartPageProperty); }
            set { SetValue(StartPageProperty, value); }
        }

        private void OnStartPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {

        }

        #endregion
    }
}
