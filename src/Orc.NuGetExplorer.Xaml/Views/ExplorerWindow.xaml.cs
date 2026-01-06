namespace Orc.NuGetExplorer.Views;

using System.Windows;
using Catel.MVVM.Views;

/// <summary>
/// Interaction logic for ExplorerWindow.xaml
/// </summary>
internal partial class ExplorerWindow
{
    partial void OnInitializedComponent()
    {
        SetCurrentValue(ShowInTaskbarProperty, false);
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        var screenHeight = SystemParameters.PrimaryScreenHeight;
        TopGrid.SetCurrentValue(HeightProperty, screenHeight * 2 / 3);

        SetCurrentValue(TitleProperty, (string?)ViewModel?.Title);
    }

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
        DependencyProperty.Register(nameof(StartPage), typeof(string), typeof(ExplorerWindow), new PropertyMetadata("Browse", (s, e) => ((ExplorerWindow)s).OnStartPageChanged(s, e)));

    private void OnStartPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        // Property changed callback
    }
}
