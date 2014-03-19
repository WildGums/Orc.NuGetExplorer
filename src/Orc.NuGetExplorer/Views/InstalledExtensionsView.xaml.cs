namespace Orc.NuGetExplorer.Views
{
    using System.Windows;
    using Catel.MVVM.Views;
    using Catel.Windows.Controls;

    /// <summary>
    /// Interaction logic for InstalledExtensionsView.xaml.
    /// </summary>
    public partial class InstalledExtensionsView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstalledExtensionsView"/> class.
        /// </summary>
        public InstalledExtensionsView()
        {
            InitializeComponent();
        }

        #region Properties
        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string PackageSource
        {
            get { return (string)GetValue(PackageSourceProperty); }
            set { SetValue(PackageSourceProperty, value); }
        }

        public static readonly DependencyProperty PackageSourceProperty = DependencyProperty.Register("PackageSource", typeof(string),
            typeof(InstalledExtensionsView), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        #endregion
    }
}
