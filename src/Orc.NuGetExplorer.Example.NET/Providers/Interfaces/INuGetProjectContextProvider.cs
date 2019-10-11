using NuGet.ProjectManagement;

namespace Orc.NuGetExplorer.Providers
{
    public interface INuGetProjectContextProvider
    {
        INuGetProjectContext GetProjectContext(FileConflictAction fileConflictAction);
    }
}