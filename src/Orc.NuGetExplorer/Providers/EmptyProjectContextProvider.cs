namespace Orc.NuGetExplorer.Providers;

using NuGet.ProjectManagement;

public class EmptyProjectContextProvider : INuGetProjectContextProvider
{
    public INuGetProjectContext? GetProjectContext(FileConflictAction fileConflictAction)
    {
        return new EmptyNuGetProjectContext();
    }
}
