namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NuGet.Configuration;

    public static class DefaultNuGetComparers
    {
        static DefaultNuGetComparers()
        {
            PackageSource = new PackageSourceEqualityComparer();
        }


        public static IEqualityComparer<PackageSource> PackageSource { get; set; }

        /// <summary>
        /// Custom equality comparer, comparing PackageSources by source string
        /// </summary>
        private class PackageSourceEqualityComparer : IEqualityComparer<PackageSource>
        {
            public bool Equals(PackageSource x, PackageSource y)
            {
                return string.Equals(x.Source, y.Source);
            }

            public int GetHashCode(PackageSource obj)
            {
                return obj.Source.GetHashCode();
            }
        }
    }
}
