namespace Orc.NuGetExplorer
{
    public class ExplorerTab
    {
        public static ExplorerTab Browse = new ExplorerTab("Browse");

        public static ExplorerTab Update = new ExplorerTab("Updates");

        public static ExplorerTab Installed = new ExplorerTab("Installed");

        private ExplorerTab(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
