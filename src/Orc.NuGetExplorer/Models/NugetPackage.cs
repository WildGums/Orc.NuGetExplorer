using Catel.Data;
using Catel.Logging;
using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orc.NuGetExplorer.Enums;

namespace Orc.NuGetExplorer.Models
{
    public class NuGetPackage : ModelBase
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IPackageSearchMetadata _packageMetadata;

        private readonly IList<IPackageSearchMetadata> _additionalMetadata = new List<IPackageSearchMetadata>();

        public NuGetPackage(IPackageSearchMetadata packageMetadata)
        {
            _packageMetadata = packageMetadata;
            Title = packageMetadata.Title;
            Description = packageMetadata.Description;
            IconUrl = packageMetadata.IconUrl;
            Authors = packageMetadata.Authors;
            DownloadCount = packageMetadata.DownloadCount;
            Summary = packageMetadata.Summary;

            LastVersion = packageMetadata.Identity.Version;
        }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Authors { get; private set; }

        public long? DownloadCount { get; private set; }

        public string Summary { get; private set; }

        public Uri IconUrl { get; private set; }

        public PackageStatus Status { get; private set; } = PackageStatus.NotInstalled;

        public PackageIdentity Identity => _packageMetadata.Identity;

        private List<NuGetVersion> _versions = new List<NuGetVersion>();
        public IReadOnlyList<NuGetVersion> Versions
        {
            get { return _versions; }
            private set
            {
                _versions = value.ToList();
            }
        }

        public IEnumerable<VersionInfo> VersionsInfo { get; private set; } = new List<VersionInfo>();

        public bool IsLoaded { get; private set; }

        public NuGetVersion LastVersion { get; private set; }

        public NuGetVersion InstalledVersion { get; set; }

        public async Task MergeMetadata(IPackageSearchMetadata searchMetadata, MetadataOrigin pageToken)
        {
            _additionalMetadata.Add(searchMetadata);

            //merge versions
            var versInfo = await searchMetadata.GetVersionsAsync();

            if (!Versions.Any())
            {
                //currently package versions doesn't loaded from metadata,
                //but we still can add current version to list and check it as our LastVersion

                var singleVersion = searchMetadata.Identity.Version;
                _versions.Add(singleVersion);
            }

            if (versInfo != null)
            {
                VersionsInfo = VersionsInfo.Union(versInfo).Distinct();

                Versions = VersionsInfo.Select(x => x.Version).OrderByDescending(x => x).ToList();
            }

            LastVersion = Versions?.FirstOrDefault() ?? Identity.Version;
        }

        public async Task<IEnumerable<NuGetVersion>> LoadVersionsAsync()
        {
            if (IsLoaded)
            {
                return null;
            }

            var versinfo = await _packageMetadata.GetVersionsAsync();

            var versions = versinfo.Select(x => x.Version).Union(Versions).OrderByDescending(x => x)
                .ToList();

            _versions.Clear();

            _versions.AddRange(versions);

            VersionsInfo = versinfo;

            IsLoaded = true;

            return Versions;
        }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (_packageMetadata == null)
            {
                return;
            }

            if (String.Equals(e.PropertyName, nameof(Status)))
            {
                Log.Info($"{Identity} status was changed from {e.OldValue} to {e.NewValue}");
            }
        }
    }
}
