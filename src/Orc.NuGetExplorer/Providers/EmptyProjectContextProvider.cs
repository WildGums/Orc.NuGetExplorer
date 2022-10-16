namespace Orc.NuGetExplorer.Providers
{
    using NuGet.ProjectManagement;

    public class EmptyProjectContextProvider : INuGetProjectContextProvider
    {
        public INuGetProjectContext? GetProjectContext(FileConflictAction fileConflictAction)
        {
            //no project context
            return null;
        }
    }
}
