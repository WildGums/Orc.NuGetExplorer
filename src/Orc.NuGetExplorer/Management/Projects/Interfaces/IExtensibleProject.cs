namespace Orc.NuGetExplorer
{
    using NuGet.Packaging.Core;

    public interface IExtensibleProject
    {
        string Name { get; }

        string Framework { get; }

        string ContentPath { get; }

        string GetInstallPath(PackageIdentity packageIdentity);

        void Install();

        void Update();

        void Uninstall();
    }
}
