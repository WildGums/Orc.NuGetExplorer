namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        #region Properties
        bool IsEnabled { get; }
        bool IsOfficial { get; }
        string Name { get; }
        string Source { get; }
        #endregion
    }
}