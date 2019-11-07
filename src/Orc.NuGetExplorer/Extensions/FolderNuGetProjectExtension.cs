namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NuGet.ProjectManagement;

    public static class FolderNuGetProjectExtension
    {
        public static IEnumerable<string> GetPackageDirectories(this FolderNuGetProject project)
        {
            var packageDirs = Directory.GetDirectories(project.Root);

            return packageDirs;
        }
    }
}
