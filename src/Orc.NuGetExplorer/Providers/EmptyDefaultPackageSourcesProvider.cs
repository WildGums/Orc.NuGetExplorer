namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;

    public class EmptyDefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        #region Properties
        public string DefaultSource { get; set; }
        #endregion

        #region Methods
        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return Enumerable.Empty<IPackageSource>();
        }
        #endregion
    }
}
