namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows.Controls;
    using NuGetExplorer.ViewModels;

    /// <summary>
    /// Interaction logic for FeedDetailView.xaml
    /// </summary>
    internal partial class FeedDetailView : UserControl
    {
        public FeedDetailView()
        {
            InitializeComponent();
        }

        public FeedDetailView(FeedDetailViewModel viewModel)
            : base(viewModel)
        {
            InitializeComponent();
        }
    }
}
