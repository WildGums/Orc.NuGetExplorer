// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Catel;
    using Catel.Data;

    using NuGet;

    internal class PackageDetails : ModelBase, IPackageDetails
    {
        private readonly IEnumerable<string> _availableVersionsEnumeration;

        private IList<string> _availableVersions;

        #region Constructors
        internal PackageDetails(IPackage package, IEnumerable<string> availableVersions)
        {
            Argument.IsNotNull(() => package);
            Argument.IsNotNull(() => availableVersions);

            Package = package;
            Version = package.Version.Version;

            Id = package.Id;
            Title = string.IsNullOrWhiteSpace(package.Title) ? package.Id : package.Title;
            FullName = package.GetFullName();
            Description = package.Description;
            IconUrl = package.IconUrl;

            Published = package.Published?.LocalDateTime;
            SpecialVersion = package.Version.SpecialVersion;
            IsAbsoluteLatestVersion = package.IsAbsoluteLatestVersion;

            ValidationContext = new ValidationContext();
            IsPrerelease = !string.IsNullOrWhiteSpace(SpecialVersion);

            _availableVersionsEnumeration = availableVersions;
        }

        #endregion

        #region Properties
        public string SelectedVersion { get; set; }

        public IValidationContext ValidationContext { get; private set; }

        public string Id { get; }

        public string Title { get; }

        public IEnumerable<string> Authors => Package.Authors;

        DateTimeOffset? IPackageDetails.Published
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage == null ? Published : dataServicePackage.Published;
            }
        }

        public int? DownloadCount
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage?.DownloadCount;
            }
        }

        public string Dependencies
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage?.Dependencies;
            }
        }

        public bool? IsInstalled { get; set; }

        public IList<string> AvailableVersions
        {
            get
            {
                if (_availableVersions == null)
                {
                    _availableVersions = _availableVersionsEnumeration.OrderByDescending(version => 
                        version.Contains("-")
                            ? version.Replace("-unstable", "-1").Replace("-beta", "-3").Replace("-alpha", "-2") : 
                            version + "-4")
                        .Distinct().Take(Settings.NuGet.PackageCount).ToList();

                    SelectedVersion = _availableVersions.FirstOrDefault(version => !version.Contains("-")) ?? _availableVersions.FirstOrDefault();
                }

                return _availableVersions;
            }
        }

        public string FullName { get; }

        public string Description { get; }

        public Uri IconUrl { get; }

        internal IPackage Package { get; }

        public DateTime? Published { get; }

        public Version Version { get; }

        public string SpecialVersion { get; }

        public bool IsAbsoluteLatestVersion { get; }

        public bool IsLatestVersion { get; private set; }

        public bool IsPrerelease { get; }
        #endregion

        #region Methods
        public void ResetValidationContext()
        {
            ValidationContext = new ValidationContext();
        }
        #endregion
    }
}
