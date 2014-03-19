
namespace Orc.NuGetExplorer.Services
{
    using System.Collections.ObjectModel;
    using NuGet;

    public interface IPackageSourceService
    {
        IPackageRepository GetPackageRepository(string packageSource);
        void Save();
        void Load();
        ObservableCollection<PackageSource> PackageSources { get; }
    }
}