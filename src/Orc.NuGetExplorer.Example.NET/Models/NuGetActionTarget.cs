namespace Orc.NuGetExplorer.Models
{
    using Catel.Data;
    using System.Collections.Generic;

    public class NuGetActionTarget : ModelBase
    {
        private List<IExtensibleProject> extensibleProjects = new List<IExtensibleProject>();

        public IReadOnlyList<IExtensibleProject> TargetProjects => extensibleProjects;

        public void Add(IExtensibleProject project)
        {
            extensibleProjects.Add(project);

            RaisePropertyChanged(() => TargetProjects);
        }

        public void Remove(IExtensibleProject project)
        {
            extensibleProjects.Remove(project);

            RaisePropertyChanged(() => TargetProjects);
        }

        public bool IsValid { get; private set; }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (string.Equals(e.PropertyName, nameof(TargetProjects)))
            {
                IsValid = extensibleProjects.Count > 0;
            }
        }
    }
}
