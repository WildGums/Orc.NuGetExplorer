namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;

    public sealed class CombinedNuGetSource : INuGetSource
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly List<INuGetSource> _sourceList = new();

        public CombinedNuGetSource(IReadOnlyList<INuGetSource> feedList)
        {
            foreach (var feed in feedList)
            {
                if (feed is CombinedNuGetSource)
                {
                    throw Log.ErrorAndCreateException<InvalidOperationException>("Nested multiple source feeds are not allowed");
                }
                _sourceList.Add(feed);
            }
        }

        public string Name => Constants.CombinedSourceName;

        public string Source => _sourceList.FirstOrDefault()?.Source ?? string.Empty;

        public bool IsAccessible => IsAllFeedsAccessible();

        public bool IsVerified => IsAllVerified();

        public bool IsEnabled => true;

        public bool IsOfficial { get; set; }

        public bool IsSelected { get; set; }

        public void AddFeed(NuGetFeed feed)
        {
            _sourceList.Add(feed);
        }

        public void RemoveFeed(NuGetFeed feed)
        {
            _sourceList.Remove(feed);
        }

        public IEnumerable<NuGetFeed> GetAllSources()
        {
            return _sourceList.Select(x => x as NuGetFeed).Where(x => x is not null).ToList()!;
        }

        public override string ToString()
        {
            return Name;
        }

        public PackageSourceWrapper GetPackageSource()
        {
            return new PackageSourceWrapper(_sourceList.Select(x => x.Source).ToList());
        }

        public bool IsAllFeedsAccessible()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsAccessible);
        }

        public bool IsAllVerified()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsVerified);
        }
    }
}
