namespace Orc.NuGetExplorer
{
    public interface IExtensibleProject
    {
        string Name { get; }

        string Framework { get; }

        string ContentPath { get; }

        void Install();

        void Update();

        void Uninstall();
    }
}
