namespace Orc.NuGetExplorer;

public interface INuGetSource : IPackageSource
{
    bool IsAccessible { get; }

    bool IsVerified { get; }

    bool IsSelected { get; set; }

    PackageSourceWrapper GetPackageSource();
}