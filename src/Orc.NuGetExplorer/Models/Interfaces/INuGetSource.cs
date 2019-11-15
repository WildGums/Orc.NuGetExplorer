namespace Orc.NuGetExplorer.Models
{
    public interface INuGetSource : IPackageSource
    {
        PackageSourceWrapper GetPackageSource();

        bool IsAccessible { get; }

        bool IsVerified { get; }

        bool IsSelected { get; set; }
    }
}
