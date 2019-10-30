namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using Catel.Data;
    using Catel.MVVM;
    using NuGet.Packaging;

    public class DependenciesViewModel : ViewModelBase
    {
        /// <summary>
        /// This is property inside child viewmodel mapped via attribute
        /// </summary>
        public object Collection { get; set; }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(Collection)))
            {
                HasDependency = ((Collection as List<PackageDependencyGroup>)?.Count ?? 0) > 0;
            }
        }

        public bool HasDependency { get; set; }
    }
}
