namespace Orc.NuGetExplorer.Views
{
    using NuGetExplorer.ViewModels;
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for FeedDetailView.xaml
    /// </summary>
    internal partial class FeedDetailView : UserControl
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
