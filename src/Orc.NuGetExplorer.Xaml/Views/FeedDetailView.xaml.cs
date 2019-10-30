namespace Orc.NuGetExplorer.Views
{
    using NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for FeedDetailView.xaml
    /// </summary>
    public partial class FeedDetailView : Catel.Windows.Controls.UserControl
    {
        public FeedDetailView()
        {
            InitializeComponent();
        }

        public FeedDetailView(FeedDetailViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
