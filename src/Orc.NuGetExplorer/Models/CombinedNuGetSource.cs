namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CombinedNuGetSource : INuGetSource
    {
        private List<INuGetSource> _sourceList = new List<INuGetSource>();

        public CombinedNuGetSource(IReadOnlyList<INuGetSource> feedList)
        {
            foreach (var feed in feedList)
            {
                if (feed is CombinedNuGetSource)
                {
                    throw new InvalidOperationException("NuGet Source with multiple feeds cannot contains сontains nested sources");
                }
                _sourceList.Add(feed);
            }
        }

        public string Name => "All";

        public string Source => _sourceList.FirstOrDefault()?.Source;//returns top source

        public bool IsAccessible => IsAllFeedsAccessible();

        public bool IsVerified => IsAllVerified();

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
            return _sourceList.Select(x => x as NuGetFeed);
        }

        public override string ToString()
        {
            return Name;
        }

        public PackageSourceWrapper GetPackageSource()
        {
            return new PackageSourceWrapper(_sourceList.Select(x => x.Source).ToList());
        }

        protected bool IsAllFeedsAccessible()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsAccessible);
        }

        protected bool IsAllVerified()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsVerified);
        }
    }
}
