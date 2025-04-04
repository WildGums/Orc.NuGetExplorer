﻿namespace Orc.NuGetExplorer.Example;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Catel.IoC;
using Catel.Logging;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using Orc.NuGetExplorer.Example.Packaging;

public class ExampleProject : IExtensibleProject
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();
    private readonly ExamplePackagePathResolver _pathResolver;

    public ExampleProject(IFrameworkNameProvider frameworkNameProvider)
    {
        ArgumentNullException.ThrowIfNull(frameworkNameProvider);

        _pathResolver = TypeFactory.Default.CreateInstanceWithParametersAndAutoCompletion<ExamplePackagePathResolver>();

        var targetFramework = FrameworkParser.TryParseFrameworkName(Framework, frameworkNameProvider);
        SupportedPlatforms = [FrameworkParser.ToSpecificPlatform(targetFramework)];
    }

    public string Name => "Example";

    public string Framework => ".NETCoreApp,Version=v8.0";

    public string ContentPath => _pathResolver.AppRootDirectory;

    public IReadOnlyList<NuGetFramework> SupportedPlatforms { get; set; }

    public bool IgnoreDependencies { get { return IgnoreMissingDependencies; } }

    public bool IgnoreMissingDependencies { get; } = false;

    public bool SupportSideBySide { get; } = true;

    public bool NoCache { get; } = false;

    public string GetInstallPath(PackageIdentity packageIdentity)
    {
        return _pathResolver.GetInstallPath(packageIdentity);
    }

    public PackagePathResolver GetPathResolver()
    {
        return _pathResolver;
    }

    public void Install()
    {
        Log.Info("Installation started");
    }

    public void Uninstall()
    {
        Log.Info("Uninstall started");
    }

    public void Update()
    {
        Log.Info("Update started");
    }

    public override string ToString()
    {
        return Name;
    }
}
