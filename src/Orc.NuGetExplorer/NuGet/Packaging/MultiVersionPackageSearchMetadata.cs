namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    /// <summary>
    /// Aggregate package metadata containing preloaded informations about versions
    /// </summary>
    internal class MultiVersionPackageSearchMetadata : ClonedPackageSearchMetadata, IPackageDetails
    {
        private readonly List<string> _availableVersions = new List<string>();
        private string _overridenVersion = null;

        public MultiVersionPackageSearchMetadata(IEnumerable<IPackageSearchMetadata> availableVersions)
        {
            _availableVersions.AddRange(availableVersions.Select(x => x.Identity.Version.ToFullString()));
        }

        public string Id => Identity.Id;

        public string FullName => Identity?.ToFullString();

        public Version Version => Identity.Version.Version;

        public NuGetVersion NuGetVersion => Identity.Version;

        public string SpecialVersion => String.Empty;

        //todo
        public bool IsAbsoluteLatestVersion { get; }

        //todo
        public bool IsLatestVersion { get; }

        public bool IsPrerelease => Identity.Version.IsPrerelease;

        //todo
        public string Dependencies { get; }

        public bool? IsInstalled { get; set; }

        public IList<string> AvailableVersions => _availableVersions;

        public string SelectedVersion
        {
            get { return _overridenVersion ?? Identity?.Version?.ToString(); } //selected version by default is version from identity
            set
            {
                _overridenVersion = value;
            }
        }

        public IValidationContext ValidationContext { get; private set; } = new ValidationContext();

        IEnumerable<string> IPackageDetails.Authors => Authors.SplitOrEmpty();

        int? IPackageDetails.DownloadCount => (int?)DownloadCount;

        public PackageIdentity GetIdentity()
        {
            return Identity;
        }

        public void ResetValidationContext()
        {
            ValidationContext = new ValidationContext();
        }
    }
}
