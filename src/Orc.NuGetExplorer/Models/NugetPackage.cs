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

    public sealed class NuGetPackage : ModelBase, IPackageDetails, IObservablePackage
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
            SelectedVersion = LastVersion.ToFullString();

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
                    Status = PackageStatus.UpdateAvailable;
                    break;
            }
        }

        public static event EventHandler? AnyNuGetPackageCheckedChanged;
        public event EventHandler<PackageModelStatusEventArgs>? StatusChanged;

        private bool _isChecked;
        public bool IsChecked
        {
            get
            {
                if (IsDelisted)
                {
                    return false;
                }

                return _isChecked;
            }
            set
            {
                if (value != _isChecked)
                {
                    _isChecked = value;
                    RaisePropertyChanged(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        public MetadataOrigin FromPage { get; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public string Authors { get; private set; }

        public string Summary { get; private set; }

        public Uri IconUrl { get; private set; }

        private PackageStatus _status = PackageStatus.NotInstalled;
        public PackageStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    var oldValue = _status;
                    _status = value;
                    RaisePropertyChanged(this, new PropertyChangedExtendedEventArgs<PackageStatus>(oldValue, _status));
                }
            }
        }

        public PackageIdentity Identity => _packageMetadata.Identity;

        private List<NuGetVersion> _versions = new();

        public IReadOnlyList<NuGetVersion> Versions
        {
            get
            {
                return _versions;
            }
            private set
            {
                _versions = value.ToList();
            }
        }

        public IEnumerable<VersionInfo> VersionsInfo { get; private set; } = new List<VersionInfo>();

        public bool IsLoaded { get; private set; }

        public bool IsDelisted { get; set; }

        public NuGetVersion LastVersion { get; private set; }

        /// <summary>
        /// Installed version or null if it's Browse'' model
        /// </summary>
        public NuGetVersion? InstalledVersion { get; set; }

        #region IPackageDetails

        public string Id => Identity?.Id ?? string.Empty;

        public string FullName => $"{Id} {Identity.Version.ToFullString()}";

        public Version Version => Identity.Version.Version;

        public NuGetVersion NuGetVersion => Identity.Version;

        public bool IsLatestVersion => Identity?.Version.Equals(LastVersion) ?? false;

        public bool IsPrerelease => Identity?.Version.IsPrerelease ?? false;

        public bool? IsInstalled { get; set; }

        public string SelectedVersion { get; set; }

        public IValidationContext? ValidationContext { get; set; }

        IEnumerable<string> IPackageDetails.Authors => Authors.SplitOrEmpty();

        public int? DownloadCount { get; private set; }

        public DateTimeOffset? Published => _packageMetadata.Published;

        public void ResetValidationContext()
        {
            ValidationContext = new ValidationContext();
        }

        #endregion

        public async Task MergeMetadataAsync(IPackageSearchMetadata searchMetadata, MetadataOrigin pageToken)
        {
            try
            {
                // TODO: revise this part

                _additionalMetadata.Add(searchMetadata);

                //merge versions
                var versInfo = await searchMetadata.GetVersionsAsync();

                if (!Versions.Any())
                {
                    // currently package versions doesn't loaded from metadata,
                    // but we still can add current version to list and check it as our LastVersion

                    var singleVersion = searchMetadata.Identity.Version;
                    _versions.Add(singleVersion);
                }

                // add basic versions
                var basicVersions = new VersionInfo[]
                {
                    new VersionInfo(searchMetadata.Identity.Version),
                    new VersionInfo(_packageMetadata.Identity.Version)
                };

                VersionsInfo = VersionsInfo.Union(basicVersions).Distinct();

                if (versInfo is not null)
                {

                    VersionsInfo = VersionsInfo.Union(versInfo).Distinct();

                    Versions = VersionsInfo.Select(x => x.Version).OrderByDescending(x => x).ToList();
                }

                LastVersion = Versions?.FirstOrDefault() ?? Identity.Version;
            }
            catch (NullReferenceException ex)
            {
                Log.Warning(ex, $"possibly because package available only from local source or local package {searchMetadata.Identity} installation is missed or corrupted");
            }
        }

        public async Task<IEnumerable<NuGetVersion>?> LoadVersionsAsync()
        {
            if (IsLoaded)
            {
                // TODO: checkout is it should be cached value returned?
                return null;
            }

            //Error on v2 feed
            var versinfo = await _packageMetadata.GetVersionsAsync();

            //Workaround for Updates metadata
            if (!versinfo.Any() && _packageMetadata is UpdatePackageSearchMetadata updateMetadata)
            {
                versinfo = await updateMetadata.LazyVersionsFactory;
            }

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

        public IPackageSearchMetadata GetMetadata()
        {
            return _packageMetadata;
        }

        public PackageIdentity GetIdentity()
        {
            return Identity;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (_packageMetadata is null)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(Status)))
            {
                if (e.HasPropertyChanged(nameof(Status)))
                {
                    if (e is PropertyChangedExtendedEventArgs<PackageStatus> statusChangedArgs)
                    {
                        RaiseStatusChanged(statusChangedArgs.OldValue, statusChangedArgs.NewValue);
                        Log.Info($"{Identity} status was changed from {statusChangedArgs.OldValue} to {statusChangedArgs.NewValue}");
                    }
                }
            }
        }

        private void OnIsCheckedChanged()
        {
            AnyNuGetPackageCheckedChanged?.Invoke(this, EventArgs.Empty);
        }

        private void RaiseStatusChanged(PackageStatus oldValue, PackageStatus newValue)
        {
            StatusChanged?.Invoke(this, new PackageModelStatusEventArgs(oldValue, newValue));
        }
    }
}
