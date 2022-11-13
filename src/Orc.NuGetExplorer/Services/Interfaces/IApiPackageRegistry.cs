namespace Orc.NuGetExplorer
{
    public interface IApiPackageRegistry
    {
        void Register(string packageName, string version);

        bool IsRegistered(string packageName);

        void Validate(IPackageDetails package);
    }
}
