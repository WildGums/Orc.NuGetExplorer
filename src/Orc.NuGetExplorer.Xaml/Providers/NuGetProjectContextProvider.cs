namespace Orc.NuGetExplorer.Providers
{
    using Catel;
    using Catel.IoC;
    using NuGet.ProjectManagement;
    using Orc.NuGetExplorer.Management;


    public class NuGetProjectContextProvider : INuGetProjectContextProvider
    {
        private readonly ITypeFactory _typeFactory;

        public NuGetProjectContextProvider(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }

        public INuGetProjectContext GetProjectContext(FileConflictAction fileConflictAction)
        {
            var projectContext = _typeFactory.CreateInstanceWithParametersAndAutoCompletion<NuGetProjectContext>(fileConflictAction);

            return projectContext;
        }
    }
}
