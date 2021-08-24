namespace Orc.NuGetExplorer
{
    public class ExplorerTab
    {
        public readonly static ExplorerTab Browse = new(ExplorerPageName.Browse);

        public readonly static ExplorerTab Update = new(ExplorerPageName.Updates);

        public readonly static ExplorerTab Installed = new(ExplorerPageName.Installed);

        private ExplorerTab(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
