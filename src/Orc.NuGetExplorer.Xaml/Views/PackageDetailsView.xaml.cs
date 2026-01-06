namespace Orc.NuGetExplorer.Views;

using System.Windows;
using Catel;
using Catel.IoC;
using Catel.MVVM.Views;

internal partial class PackageDetailsView
{
    [ViewToViewModel(MappingType = ViewToViewModelMappingType.ViewToViewModel)]
    public NuGetPackage? Package
    {
        get { return (NuGetPackage?)GetValue(PackageProperty); }
        set { SetValue(PackageProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Package"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty PackageProperty = DependencyProperty.Register(
        nameof(Package), typeof(NuGetPackage), typeof(PackageDetailsView), new PropertyMetadata(default(NuGetPackage)));
}
