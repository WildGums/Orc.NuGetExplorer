namespace Orc.NuGetExplorer.ViewModels;

using System.Collections.Generic;
using System.ComponentModel;
using Catel.MVVM;
using NuGet.Packaging;

internal class DependenciesViewModel : ViewModelBase
{
    /// <summary>
    /// This is property mapped via attribute
    /// </summary>
    public object? Collection { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (e.HasPropertyChanged(nameof(Collection)))
        {
            HasDependency = ((Collection as List<PackageDependencyGroup>)?.Count ?? 0) > 0;
        }
    }

    public bool HasDependency { get; set; }
}