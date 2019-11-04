namespace Orc.NuGetExplorer.Services
{
    using System.Collections.Generic;

    public interface IFileDirectoryService
    {
        void DeleteDirectoryTree(string folderPath, out List<string> failedEntries);

        string GetGlobalPackagesFolder();

        string GetApplicationRoamingFolder();

        string GetApplicationLocalFolder();
    }
}
