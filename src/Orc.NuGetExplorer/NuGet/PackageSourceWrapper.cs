namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NuGet.Configuration;

    public class PackageSourceWrapper
    {
        public static explicit operator PackageSource(PackageSourceWrapper wrapper)
        {
            if (wrapper.IsMultipleSource)
            {
                throw new InvalidCastException("Wrong casting from 'PackageSourceWrapper' to single 'PackageSource' bacuse of wrapper contains multiple sources");
            }
            return wrapper.PackageSources.FirstOrDefault();
        }

        public IReadOnlyList<PackageSource> PackageSources { get; private set; }

        public bool IsMultipleSource => PackageSources.Count > 1;

        public PackageSourceWrapper(string source)
        {
            PackageSources = new List<PackageSource>() { new PackageSource(source) };
        }

        public PackageSourceWrapper(IReadOnlyList<string> sources)
        {
            PackageSources = new List<PackageSource>(sources.Select(x => new PackageSource(x)));
        }

        public override string ToString()
        {
            return String.Join<PackageSource>("; ", PackageSources.ToArray());
        }
    }
}
