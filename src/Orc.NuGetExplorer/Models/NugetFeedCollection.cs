namespace Orc.NuGetExplorer.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class NugetFeedCollection : IPackageSource, IList<NuGetFeed>
    {
        private readonly List<NuGetFeed> _sourceList = new();
        private bool _isSelected;

        public NugetFeedCollection(IReadOnlyList<NuGetFeed> feedList, string name)
            : this(feedList)
        {
            Name = name;
        }

        public NugetFeedCollection(IReadOnlyList<NuGetFeed> feedList)
        {
            foreach (var feed in feedList)
            {
                _sourceList.Add(feed);
            }
        }

        public NuGetFeed this[int index]
        {
            get => _sourceList[index];
            set => _sourceList[index] = value;
        }

        public string Name { get; private set; } //=> Constants.CombinedSourceName;

        public string Source => _sourceList.FirstOrDefault()?.Source; //returns top source

        public bool IsAccessible => IsAllFeedsAccessible();

        public bool IsVerified => IsAllVerified();

        public bool IsEnabled => true;

        [ObsoleteEx(TreatAsErrorFromVersion = "5.0", RemoveInVersion = "5.1")]
        public bool IsOfficial { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                foreach (var referencedFeed in _sourceList)
                {
                    referencedFeed.IsSelected = value;
                }
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public PackageSourceWrapper GetPackageSource()
        {
            return new PackageSourceWrapper(_sourceList.Select(x => x.Source).ToList());
        }

        private bool IsAllFeedsAccessible()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsAccessible);
        }

        private bool IsAllVerified()
        {
            return _sourceList.Any() && _sourceList.All(x => x.IsVerified);
        }

        #region IList

        public bool IsReadOnly => false;

        public int Count => _sourceList.Count;

        public int IndexOf(NuGetFeed item)
        {
            return _sourceList.IndexOf(item);
        }

        public void Insert(int index, NuGetFeed item)
        {
            _sourceList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _sourceList.RemoveAt(index);
        }

        public void Add(NuGetFeed item)
        {
            _sourceList.Add(item);
        }

        public void Clear()
        {
            _sourceList.Clear();
        }

        public bool Contains(NuGetFeed item)
        {
            return _sourceList.Contains(item);
        }

        public void CopyTo(NuGetFeed[] array, int arrayIndex)
        {
            _sourceList.CopyTo(array, arrayIndex);
        }

        public bool Remove(NuGetFeed item)
        {
            return _sourceList.Remove(item);
        }

        public IEnumerator<NuGetFeed> GetEnumerator()
        {
            return _sourceList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
