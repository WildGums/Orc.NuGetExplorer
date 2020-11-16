namespace Orc.NuGetExplorer
{
    using System.IO;
    using NuGet.Common;

    public static class DefaultNuGetFolders
    {
        public static readonly string DefaultGlobalPackagesFolderPath = "packages" + Path.DirectorySeparatorChar;

        public static string GetGlobalPackagesFolder()
        {
            return Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), DefaultGlobalPackagesFolderPath);
        }

        public static string GetApplicationRoamingFolder()
        {
            return Catel.IO.Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming, Constants.CompanyName, Constants.ProductName);
        }

        public static string GetApplicationLocalFolder()
        {
            return Catel.IO.Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserLocal, Constants.CompanyName, Constants.ProductName);
        }
    }
}
