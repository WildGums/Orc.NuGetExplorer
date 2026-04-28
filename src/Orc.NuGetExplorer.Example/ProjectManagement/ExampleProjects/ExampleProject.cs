namespace Orc.NuGetExplorer.Example;

using System;
using System.Collections.Generic;
using Catel.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using Orc.NuGetExplorer.Example.Packaging;

public class ExampleProject : IExtensibleProject
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(ExampleProject));

    private readonly ExamplePackagePathResolver _pathResolver;

    public ExampleProject(IFrameworkNameProvider frameworkNameProvider, IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(frameworkNameProvider);

        _pathResolver = ActivatorUtilities.CreateInstance<ExamplePackagePathResolver>(serviceProvider);

        var targetFramework = FrameworkParser.TryParseFrameworkName(Framework, frameworkNameProvider);
        SupportedPlatforms = [FrameworkParser.ToSpecificPlatform(targetFramework)];
    }

    public string Name => "Example";

    public string Framework => ".NETCoreApp,Version=v10.0";

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
        Logger.LogInformation("Installation started");
    }

    public void Uninstall()
    {
        Logger.LogInformation("Uninstall started");
    }

    public void Update()
    {
        Logger.LogInformation("Update started");
    }

    public override string ToString()
    {
        return Name;
    }
}
