namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using Orc.NuGetExplorer.Models;

    public interface IExtensibleProject
    {
        string Name { get; }

        string Framework { get; }

        string ContentPath { get; }

        IReadOnlyList<NuGetPackage> PackageList { get; }

        void Install();

        void Update();

        void Uninstall();
    }
}
