namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;
    using NuGet.Configuration;

    public class PackageSourceWrapper
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static explicit operator PackageSource(PackageSourceWrapper wrapper)
        {
            ArgumentNullException.ThrowIfNull(wrapper);

            if (wrapper.IsMultipleSource || !wrapper.PackageSources.Any())
            {
                throw Log.ErrorAndCreateException<InvalidCastException>("Wrong casting from 'PackageSourceWrapper' to single 'PackageSource' because of wrapper containing multiple sources");
            }
            return wrapper.PackageSources[0];
        }

        public IReadOnlyList<PackageSource> PackageSources { get; private set; }

        public bool IsMultipleSource => PackageSources.Count > 1;

        public PackageSourceWrapper(string source)
        {
            PackageSources = new List<PackageSource>()
            {
                new PackageSource(source)
            };
        }

        public PackageSourceWrapper(IReadOnlyList<string> sources)
        {
            PackageSources = new List<PackageSource>(sources.Select(x => new PackageSource(x)));
        }

        public override string ToString()
        {
            return string.Join<PackageSource>("; ", PackageSources.ToArray());
        }
    }
}
