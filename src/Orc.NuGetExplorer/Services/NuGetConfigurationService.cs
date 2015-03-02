namespace Orc.NuGetExplorer
{
    using Catel.IO;

    public class NuGetConfigurationService : INuGetConfigurationService
    {
        public string GetDestinationFolder()
        {
            // TODO: this is temporary decision
            var applicationDataDirectory = Path.GetApplicationDataDirectory();
            var path = Path.Combine(applicationDataDirectory, "plugins");
            return path;
        }
    }
}