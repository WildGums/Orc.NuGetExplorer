namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IPackagesUpdatesSearcherService
    {
        IEnumerable<IPackageDetails> SearchForUpdates(bool allowPrerelease, bool authenticateIfRequired = true);
    }
}