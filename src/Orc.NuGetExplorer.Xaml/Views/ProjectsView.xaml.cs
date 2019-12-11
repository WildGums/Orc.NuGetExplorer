namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows.Controls;
    using Orc.NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for ProjectsView.xaml
    /// </summary>
    internal partial class ProjectsView : UserControl
    {
        public ProjectsView()
        {
            InitializeComponent();
        }

        public ProjectsView(ProjectsViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
