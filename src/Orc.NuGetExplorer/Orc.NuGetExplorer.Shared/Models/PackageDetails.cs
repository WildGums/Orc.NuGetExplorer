// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageDetails.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using Catel;
    using Catel.Data;
    using NuGet;

    internal class PackageDetails : ModelBase, IPackageDetails
    {
        #region Constructors
        internal PackageDetails(IPackage package)
        {
            Argument.IsNotNull(() => package);

            Package = package;
            Version = package.Version.Version;
            Id = package.Id;
            Title = string.IsNullOrWhiteSpace(package.Title) ? package.Id : package.Title;
            FullName = package.GetFullName();
            Description = package.Description;
            IconUrl = package.IconUrl;

            Published = package.Published == null ? (DateTime?) null : package.Published.Value.LocalDateTime;
            SpecialVersion = package.Version.SpecialVersion;
            IsAbsoluteLatestVersion = package.IsAbsoluteLatestVersion;

            IsPrerelease = !string.IsNullOrWhiteSpace(SpecialVersion);
        }
        #endregion

        #region Properties
        public List<string> ApiValidations { get; set; } = new List<string>();
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
                return dataServicePackage == null ? null : (int?) dataServicePackage.DownloadCount;
            }
        }

        public string Dependencies
        {
            get
            {
                var dataServicePackage = Package as DataServicePackage;
                return dataServicePackage == null ? null : dataServicePackage.Dependencies;
            }
        }

        public bool? IsInstalled { get; set; }

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
    }
}