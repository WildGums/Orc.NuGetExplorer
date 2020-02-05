namespace Orc.NuGetExplorer
{
    using NuGet.Configuration;

    public static class IRepositoryExtensions
    {
        public static PackageSource ToPackageSource(this IRepository repository)
        {
            return new PackageSource(repository.Source, repository.Name);
        }
    }
}
