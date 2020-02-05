namespace Orc.NuGetExplorer
{
    public class ExplorerTab
    {
        public static ExplorerTab Browse = new ExplorerTab(ExplorerPageName.Browse);

        public static ExplorerTab Update = new ExplorerTab(ExplorerPageName.Updates);

        public static ExplorerTab Installed = new ExplorerTab(ExplorerPageName.Installed);

        private ExplorerTab(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
