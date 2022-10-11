namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;

    public static class DefaultNuGetComparers
    {
        static DefaultNuGetComparers()
        {
            PackageSource = new PackageSourceEqualityComparer();
            SourceRepository = new UniqueSourceSourceRepositoryComparer();
        }


        public static IEqualityComparer<PackageSource> PackageSource { get; set; }

        public static IEqualityComparer<SourceRepository> SourceRepository { get; set; }

        /// <summary>
        /// Custom equality comparer, comparing PackageSources by source string
        /// </summary>
        private class PackageSourceEqualityComparer : IEqualityComparer<PackageSource>
        {
            public bool Equals(PackageSource? x, PackageSource? y)
            {
                ArgumentNullException.ThrowIfNull(x);
                ArgumentNullException.ThrowIfNull(y);

                return string.Equals(x.Source, y.Source);
            }

            public int GetHashCode(PackageSource obj)
            {
                return obj.Source.GetHashCode();
            }
        }

        /// <summary>
        /// Compare repository by sources
        /// </summary>
        private class UniqueSourceSourceRepositoryComparer : IEqualityComparer<SourceRepository>
        {
            public bool Equals(SourceRepository? x, SourceRepository? y)
            {
                ArgumentNullException.ThrowIfNull(x);
                ArgumentNullException.ThrowIfNull(y);

                return PackageSource.Equals(x.PackageSource, y.PackageSource);
            }

            public int GetHashCode(SourceRepository obj)
            {
                return PackageSource.GetHashCode(obj.PackageSource);
            }
        }
    }
}
