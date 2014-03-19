namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for UpdateExtensionsView.xaml.
    /// </summary>
    public partial class UpdateExtensionsView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateExtensionsView"/> class.
        /// </summary>
        public UpdateExtensionsView()
        {
            InitializeComponent();
        }

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string)GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }

        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(string),
            typeof(UpdateExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}
