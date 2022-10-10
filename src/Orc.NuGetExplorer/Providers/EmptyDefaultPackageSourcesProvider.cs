namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.Linq;

    public class EmptyDefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        public string DefaultSource { get; set; }

        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return Enumerable.Empty<IPackageSource>();
        }
    }
}
