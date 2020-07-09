namespace Orc.NuGetExplorer.Views
{
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for PageActionBar.xaml
    /// </summary>
    internal partial class PageActionBar : UserControl
    {
        public PageActionBar()
        {
            InitializeComponent();
        }

        private void OnSelectAllPackagesTextBlockMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CheckAllBox.SetCurrentValue(System.Windows.Controls.Primitives.ToggleButton.IsCheckedProperty, !CheckAllBox.IsChecked);
        }
    }
}
