namespace Orc.NuGetExplorer
{
    using Catel.IO;

    public class NuGetConfigurationService : INuGetConfigurationService
    {
        public string GetDestinationFolder()
        {
            var applicationDataDirectory = Path.GetApplicationDataDirectory();
            var path = Path.Combine(applicationDataDirectory, "plugins");
            return path;
        }
    }
}