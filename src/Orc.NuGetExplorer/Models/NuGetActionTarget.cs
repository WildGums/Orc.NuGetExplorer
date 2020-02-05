namespace Orc.NuGetExplorer.Models
{
    using System.Collections.Generic;
    using Catel.Data;

    public class NuGetActionTarget : ModelBase
    {
        private readonly List<IExtensibleProject> _extensibleProjects = new List<IExtensibleProject>();

        public IReadOnlyList<IExtensibleProject> TargetProjects => _extensibleProjects;

        /// <summary>
        /// Disable ability to see and change target project for commands
        /// </summary>
        public bool IsTargetProjectCanBeChanged => false;

        public bool IsValid { get; private set; }

        public void Add(IExtensibleProject project)
        {
            _extensibleProjects.Add(project);

            RaisePropertyChanged(() => TargetProjects);
        }

        public void Remove(IExtensibleProject project)
        {
            _extensibleProjects.Remove(project);

            RaisePropertyChanged(() => TargetProjects);
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(TargetProjects)))
            {
                IsValid = _extensibleProjects.Count > 0;
            }
        }
    }
}
