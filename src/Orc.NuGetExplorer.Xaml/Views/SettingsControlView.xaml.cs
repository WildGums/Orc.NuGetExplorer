namespace Orc.NuGetExplorer.Views
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Catel.MVVM.Views;
    using NuGetExplorer.ViewModels;

    /// <summary>
    /// Логика взаимодействия для SettingsControlView.xaml
    /// </summary>
    public partial class SettingsControlView : Catel.Windows.Controls.UserControl
    {
        static SettingsControlView()
        {
            typeof(PackageSourceSettingControl).AutoDetectViewPropertiesToSubscribe();
        }

        public SettingsControlView()
        {
            InitializeComponent();
        }

        public SettingsControlView(SettingsViewModel viewModel) : base(viewModel)
        {
            InitializeComponent();
        }

        #region DependencyProperty

        public static readonly DependencyProperty DefaultFeedProperty =
            DependencyProperty.Register("DefaultFeed", typeof(string), typeof(SettingsControlView), new PropertyMetadata(DefaultName.PackageSourceFeed));


        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultFeed
        {
            get { return (string)GetValue(DefaultFeedProperty); }
            set { SetValue(DefaultFeedProperty, value); }
        }

        public static readonly DependencyProperty DefaultSourceNameProperty =
            DependencyProperty.Register("DefaultSourceName", typeof(string), typeof(SettingsControlView), new PropertyMetadata(DefaultName.PackageSourceName));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
        public string DefaultSourceName
        {
            get { return (string)GetValue(DefaultSourceNameProperty); }
            set { SetValue(DefaultSourceNameProperty, value); }
        }

        public static readonly DependencyProperty PackageSourcesProperty =
            DependencyProperty.Register("PackageSources", typeof(IEnumerable<IPackageSource>), typeof(SettingsControlView), 
                new PropertyMetadata(Enumerable.Empty<IPackageSource>()));

        [ViewToViewModel(MappingType = ViewToViewModelMappingType.TwoWayViewWins)]
        public IEnumerable<IPackageSource> PackageSources
        {
            get { return (IEnumerable<IPackageSource>)GetValue(PackageSourcesProperty); }
            set { SetValue(PackageSourcesProperty, value); }
        }

        #endregion
    }
}
