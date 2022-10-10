namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface IDefaultPackageSourcesProvider
    {
        #region Properties
        string DefaultSource { get; set; }
        #endregion

        #region Methods
        IEnumerable<IPackageSource> GetDefaultPackages();
        #endregion
    }
}
