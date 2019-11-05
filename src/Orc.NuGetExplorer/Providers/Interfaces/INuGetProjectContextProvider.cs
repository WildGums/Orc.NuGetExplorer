namespace Orc.NuGetExplorer
{
    using NuGet.ProjectManagement;

    public interface INuGetProjectContextProvider
    {
        INuGetProjectContext GetProjectContext(FileConflictAction fileConflictAction);
    }
}
