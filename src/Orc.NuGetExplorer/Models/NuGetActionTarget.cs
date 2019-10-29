namespace Orc.NuGetExplorer.Models
{
    using System.Collections.Generic;
    using Catel.Data;

    public class NuGetActionTarget : ModelBase
    {
        private readonly List<IExtensibleProject> _extensibleProjects = new List<IExtensibleProject>();

        public IReadOnlyList<IExtensibleProject> TargetProjects => _extensibleProjects;

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

        public bool IsValid { get; private set; }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(TargetProjects)))
            {
                IsValid = _extensibleProjects.Count > 0;
            }
        }
    }
}
