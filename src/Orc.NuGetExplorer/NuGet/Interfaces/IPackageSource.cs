namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        bool IsEnabled { get; }
        bool IsOfficial { get; }
        string Name { get; }
        string Source { get; }
    }
}
