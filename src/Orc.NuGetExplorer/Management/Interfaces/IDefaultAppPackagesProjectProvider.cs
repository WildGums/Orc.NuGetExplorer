namespace Orc.NuGetExplorer.Management
{
    /// <summary>
    /// Provides <see cref="IExtensibleProject" /> project model.
    /// </summary>
    public interface IDefaultAppPackagesProjectProvider
    {
        IExtensibleProject GetDefaultProject();
    }
}
