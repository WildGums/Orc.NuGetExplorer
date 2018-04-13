// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageWrapper.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Versioning;
    using Catel;
    using NuGet;

    public class PackageWrapper : IPackage
    {
        private readonly IPackage _package;

        public PackageWrapper(IPackage package, IEnumerable<PackageDependencySet> dependencySets)
        {
            Argument.IsNotNull(()=> package);
            Argument.IsNotNull(()=> dependencySets);

            _package = package;
            DependencySets = dependencySets ?? package.DependencySets;
        }

        public string Id => _package.Id;

        public SemanticVersion Version => _package.Version;

        public string Title => _package.Title;

        public IEnumerable<string> Authors => _package.Authors;

        public IEnumerable<string> Owners => _package.Owners;

        public Uri IconUrl => _package.IconUrl;

        public Uri LicenseUrl => _package.LicenseUrl;

        public Uri ProjectUrl => _package.ProjectUrl;

        public bool RequireLicenseAcceptance => _package.RequireLicenseAcceptance;

        public bool DevelopmentDependency => _package.DevelopmentDependency;

        public string Description => _package.Description;

        public string Summary => _package.Summary;

        public string ReleaseNotes => _package.ReleaseNotes;

        public string Language => _package.Language;

        public string Tags => _package.Tags;

        public string Copyright => _package.Copyright;

        public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies => _package.FrameworkAssemblies;

        public ICollection<PackageReferenceSet> PackageAssemblyReferences => _package.PackageAssemblyReferences;
        public IEnumerable<PackageDependencySet> DependencySets { get; }

        public Version MinClientVersion => _package.MinClientVersion;

        public Uri ReportAbuseUrl => _package.ReportAbuseUrl;

        public int DownloadCount => _package.DownloadCount;

        public IEnumerable<IPackageFile> GetFiles()
        {
            return _package.GetFiles();
        }

        public IEnumerable<FrameworkName> GetSupportedFrameworks()
        {
            return _package.GetSupportedFrameworks();
        }

        public Stream GetStream()
        {
            return _package.GetStream();
        }

        public void ExtractContents(IFileSystem fileSystem, string extractPath)
        {
            _package.ExtractContents(fileSystem, extractPath);
        }

        public bool IsAbsoluteLatestVersion => _package.IsAbsoluteLatestVersion;

        public bool IsLatestVersion => _package.IsLatestVersion;

        public bool Listed => _package.Listed;

        public DateTimeOffset? Published => _package.Published;

        public IEnumerable<IPackageAssemblyReference> AssemblyReferences => _package.AssemblyReferences;
    }
}