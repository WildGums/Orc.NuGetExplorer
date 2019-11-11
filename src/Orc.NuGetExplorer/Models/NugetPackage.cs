namespace Orc.NuGetExplorer.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Catel.Data;
    using Catel.Logging;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using Orc.NuGetExplorer.Enums;
    using Packaging;

    public sealed class NuGetPackage : ModelBase, IPackageDetails
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IPackageSearchMetadata _packageMetadata;

        private readonly IList<IPackageSearchMetadata> _additionalMetadata = new List<IPackageSearchMetadata>();

        private readonly IDictionary<NuGetVersion, IEnumerable<PackageDependencyGroup>> _dependencyGroups = new Dictionary<NuGetVersion, IEnumerable<PackageDependencyGroup>>();

        public NuGetPackage(IPackageSearchMetadata packageMetadata, MetadataOrigin fromPage)
        {
            FromPage = fromPage;
            _packageMetadata = packageMetadata;

            Title = packageMetadata.Title;
            Description = packageMetadata.Description;
            IconUrl = packageMetadata.IconUrl;
            Authors = packageMetadata.Authors;
            DownloadCount = (int?)packageMetadata.DownloadCount;
            Summary = packageMetadata.Summary;

            LastVersion = packageMetadata.Identity.Version;

            ValidationContext = new ValidationContext();

            switch (fromPage)
            {
                case MetadataOrigin.Browse:
                    InstalledVersion = null;
                    break;

                case MetadataOrigin.Installed:
                    InstalledVersion = LastVersion;
                    break;

                case MetadataOrigin.Updates when packageMetadata is UpdatePackageSearchMetadata updatePackageSearchMetadata:
                    InstalledVersion = updatePackageSearchMetadata.FromVersion.Version;
                    break;
            }
        }

        public bool IsChecked { get; set; }

        public MetadataOrigin FromPage { get; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Authors { get; private set; }

        public string Summary { get; private set; }

        public Uri IconUrl { get; private set; }

        public PackageStatus Status { get; set; } = PackageStatus.NotInstalled;

        public PackageIdentity Identity => _packageMetadata?.Identity;

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

        #region IPackageDetails

        public string Id => Identity?.Id ?? String.Empty;

        public string FullName => $"{Id} {Identity.Version.ToFullString()}";

        public Version Version => Identity.Version.Version;

        public NuGetVersion NuGetVersion => Identity.Version;

        //todo
        public string SpecialVersion { get; set; }

        //todo
        public bool IsAbsoluteLatestVersion => IsLatestVersion;

        //todo check is comparer needed
        public bool IsLatestVersion => Identity?.Version.Equals(LastVersion) ?? false;

        public bool IsPrerelease => Identity?.Version.IsPrerelease ?? false;

        //todo
        public string Dependencies { get; set; }

        public bool? IsInstalled { get; set; }

        //todo
        public IList<string> AvailableVersions { get; set; }

        //todo
        public string SelectedVersion { get; set; }

        public IValidationContext ValidationContext { get; set; }

        IEnumerable<string> IPackageDetails.Authors => SplitAuthors(Authors);

        public int? DownloadCount { get; private set; }

        public DateTimeOffset? Published => _packageMetadata.Published;

        public void ResetValidationContext()
        {
            ValidationContext = new ValidationContext();
        }

        #endregion

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

            //Error on v2 feed
            var versinfo = await _packageMetadata.GetVersionsAsync();

            var versions = versinfo.Select(x => x.Version).Union(Versions).OrderByDescending(x => x)
                .ToList();

            _versions.Clear();

            _versions.AddRange(versions);

            VersionsInfo = versinfo;

            IsLoaded = true;

            return Versions;
        }

        public void AddDependencyInfo(NuGetVersion version, IEnumerable<PackageDependencyGroup> dependencyGroups)
        {
            if (!_dependencyGroups.ContainsKey(version))
            {
                _dependencyGroups.Add(version, dependencyGroups);
            }
        }

        public IEnumerable<PackageDependencyGroup> GetDependencyInfo(NuGetVersion version)
        {
            return _dependencyGroups.TryGetValue(version, out var groups) ? groups : new List<PackageDependencyGroup>();
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

        private IList<string> SplitAuthors(string authors)
        {
            if (!string.IsNullOrWhiteSpace(authors))
            {
                return authors.Split(',');
            }

            return new List<string>();
        }
    }
}
