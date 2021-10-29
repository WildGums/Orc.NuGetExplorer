namespace Orc.NuGetExplorer.Packaging
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Data;
    using NuGet.Packaging;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;
    using NuGet.Versioning;


    public class PackageDetails : IPackageDetails
    {
        private readonly string _authors;

        public PackageDetails(IPackageSearchMetadata metadata, bool isLatestVersion = false)
        {
            Argument.IsNotNull(() => metadata);

            Id = metadata.Identity.Id; //-V3095
            FullName = metadata.Identity?.ToFullString();
            Description = metadata.Description;
            IconUrl = metadata.IconUrl;
            NuGetVersion = metadata.Identity.Version;

            IsLatestVersion = isLatestVersion;
            Title = metadata.Title;
            _authors = metadata.Authors;
            Published = metadata.Published;
            DownloadCount = (int?)metadata.DownloadCount;

            SelectedVersion = metadata.Identity.Version.ToFullString();

            DependencySets = metadata.DependencySets ?? new List<PackageDependencyGroup>();

            ValidationContext = new ValidationContext();
        }

        public PackageDetails(IPackageSearchMetadata metadata, PackageIdentity identity, bool isLatestVersion = false)
            : this(metadata, isLatestVersion)
        {
            Argument.IsNotNull(() => metadata);
            Argument.IsNotNull(() => identity);

            Id = identity.Id;
            NuGetVersion = identity.Version;
        }

        public IEnumerable<PackageDependencyGroup> DependencySets { get; set; }

        #region IPackageDetails

        public string Id { get; }

        public string FullName { get; }

        public string Description { get; }

        public Uri IconUrl { get; }

        public Version Version => NuGetVersion?.Version;

        public NuGetVersion NuGetVersion { get; }

        public string SpecialVersion => NuGetVersion?.Release; //todo check semver 2.0

        public bool IsLatestVersion { get; }

        public bool IsPrerelease => NuGetVersion?.IsPrerelease ?? false;

        public string Title { get; }

        public IEnumerable<string> Authors => _authors.SplitIfNonEmpty();

        public DateTimeOffset? Published { get; }

        public int? DownloadCount { get; }

        public virtual string Dependencies { get; protected set; }

        public bool? IsInstalled { get; set; }

        public IList<string> AvailableVersions { get; }

        public string SelectedVersion { get; set; }

        public IValidationContext ValidationContext { get; protected set; }

        public virtual PackageIdentity GetIdentity()
        {
            return new PackageIdentity(Id, NuGetVersion);
        }

        public virtual void ResetValidationContext()
        {
            ValidationContext = new ValidationContext();
        }

        #endregion
    }
}
