namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Data;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;
    using static NuGet.Protocol.Core.Types.PackageSearchMetadataBuilder;

    /// <summary>
    /// Aggregate package metadata containing preloaded informations about versions
    /// </summary>
    internal class MultiVersionPackageSearchMetadata : ClonedPackageSearchMetadata, IPackageDetails
    {
        private readonly List<string> _availableVersions = new List<string>();

        public MultiVersionPackageSearchMetadata(IEnumerable<IPackageSearchMetadata> availableVersions)
        {
            DownloadCount = (int?)DownloadCount;
            SelectedVersion = Identity.Version.ToString(); //selcted version by default is version from identity

            _availableVersions.AddRange(availableVersions.Select(x => x.Identity.Version.ToFullString()));
        }

        public string Id => Identity.Id;

        public string FullName => $"{Id} {Identity.Version.ToFullString()}";

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

        public string SelectedVersion { get; set; }

        public IValidationContext ValidationContext { get; private set; }

        IEnumerable<string> IPackageDetails.Authors => SplitAuthors(Authors);

        int? IPackageDetails.DownloadCount { get; }

        public void ResetValidationContext()
        {
            //empty context
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
