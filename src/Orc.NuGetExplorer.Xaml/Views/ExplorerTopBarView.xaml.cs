namespace Orc.NuGetExplorer.Views;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Catel.MVVM.Views;

/// <summary>
/// Interaction logic for ExplorerTopBarView.xaml
/// </summary>
internal partial class ExplorerTopBarView : Catel.Windows.Controls.UserControl
{
    static ExplorerTopBarView()
    {
        typeof(ExplorerTopBarView).AutoDetectViewPropertiesToSubscribe();
    }

    public ExplorerTopBarView()
    {
        InitializeComponent();
    }

    #region DependencyProperty

    public TabControl? UsedOn
    {
        get { return (TabControl?)GetValue(UsedOnProperty); }
        set { SetValue(UsedOnProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="UsedOn"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty UsedOnProperty =
        DependencyProperty.Register(nameof(UsedOn), typeof(TabControl), typeof(ExplorerTopBarView), new PropertyMetadata(null));

    [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewModelToView)]
    public string StartPage
    {
        get { return (string)GetValue(StartPageProperty); }
        set { SetValue(StartPageProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="StartPage"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StartPageProperty =
        DependencyProperty.Register(nameof(StartPage), typeof(string), typeof(ExplorerTopBarView),
            new PropertyMetadata(string.Empty, (s, e) => ((ExplorerTopBarView)s).OnStartPageChanged(s, e)));


    private void OnStartPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        string selectPageWithName = e.NewValue?.ToString() ?? "Browse";

        switch (selectPageWithName)
        {
            case "Browse":
                Browse.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                return;

            case "Installed":
                Installed.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                return;

            case "Updates":
                Updates.SetCurrentValue(ToggleButton.IsCheckedProperty, true);
                return;
        }
    }

    #endregion
}