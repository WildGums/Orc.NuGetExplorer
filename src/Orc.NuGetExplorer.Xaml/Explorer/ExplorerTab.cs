namespace Orc.NuGetExplorer
{
    public class ExplorerTab
    {
        public readonly static ExplorerTab Browse = new ExplorerTab(ExplorerPageName.Browse);

        public readonly static ExplorerTab Update = new ExplorerTab(ExplorerPageName.Updates);

        public readonly static ExplorerTab Installed = new ExplorerTab(ExplorerPageName.Installed);

        private ExplorerTab(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
