namespace Orc.NuGetExplorer.Management
{
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Frameworks;

    /// <summary>
    /// Default project which represents "plugins" folder
    /// </summary>
    public class DestFolder : IExtensibleProject
    {
        public DestFolder(string destinationFolder, DefaultNuGetFramework defaultFramework)
        {
            ContentPath = destinationFolder;

            var lowest = defaultFramework.GetLowest();

            Framework = lowest.LastOrDefault()?.ToString();
        }

        public string Name => "Plugins";

        public string Framework { get; private set; }

        public IEnumerable<NuGetFramework> SupportedFrameworks { get; set; }

        public string ContentPath { get; private set; }

        public void Install()
        {

        }

        public void Uninstall()
        {

        }

        public void Update()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
