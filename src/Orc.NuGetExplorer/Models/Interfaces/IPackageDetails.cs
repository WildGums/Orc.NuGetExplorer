﻿namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using Catel.Data;
using NuGet.Packaging.Core;
using NuGet.Versioning;

public interface IPackageDetails
{
    string Id { get; }

    string FullName { get; }

    string Description { get; }

    Uri? IconUrl { get; }

    Version Version { get; }

    NuGetVersion NuGetVersion { get; }

    bool IsLatestVersion { get; }

    bool IsPrerelease { get; }

    string Title { get; }

    IEnumerable<string> Authors { get; }

    DateTimeOffset? Published { get; }

    long? DownloadCount { get; }

    bool? IsInstalled { get; set; }

    string SelectedVersion { get; set; }

    IValidationContext? ValidationContext { get; set; }

    void ResetValidationContext();

    PackageIdentity GetIdentity();
}
