namespace Orc.NuGetExplorer.Services
{
    using NuGet;

    public interface INuGetFeedService
    {
        IPackageRepository GetPackageRepository(string packageSource);
    }
}