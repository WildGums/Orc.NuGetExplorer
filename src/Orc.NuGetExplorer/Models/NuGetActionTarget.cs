namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Catel.Data;

    public class NuGetActionTarget : ModelBase
    {
        private readonly List<IExtensibleProject> _extensibleProjects = new();

        public IReadOnlyList<IExtensibleProject> TargetProjects => _extensibleProjects;

        /// <summary>
        /// Disable ability to see and change target project for commands
        /// </summary>
        public bool IsTargetProjectCanBeChanged => false;

        public bool IsValid { get; private set; }

        public void Add(IExtensibleProject project)
        {
            ArgumentNullException.ThrowIfNull(project);

            _extensibleProjects.Add(project);

            RaisePropertyChanged(nameof(TargetProjects));
        }

        public void Remove(IExtensibleProject project)
        {
            ArgumentNullException.ThrowIfNull(project);

            _extensibleProjects.Remove(project);

            RaisePropertyChanged(nameof(TargetProjects));
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(TargetProjects)))
            {
                IsValid = _extensibleProjects.Count > 0;
            }
        }
    }
}
