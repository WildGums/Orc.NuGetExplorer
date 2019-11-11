namespace Orc.NuGetExplorer.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Data;
    using Catel.MVVM;
    using NuGet.Packaging;

    internal class DependenciesViewModel : ViewModelBase
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

        private void GetAlertRecords(string title, params string[] stringLines)
        {
            Argument.IsNotNullOrWhitespace(() => title);

            /*
            if (stringLines == null)
            {
                return null;
            }

            var valuableLines = stringLines.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (!valuableLines.Any())
            {
                return null;
            }

            var inlines = valuableLines.Select(line => line.ToInline(Brushes.Red).Append(new LineBreak())).ToList();
            var inline = title.ToInline().Append(new LineBreak());
            var resultInline = inline.Bold().AppendRange(inlines);
            return resultInline;
            */
        }
    }
}
