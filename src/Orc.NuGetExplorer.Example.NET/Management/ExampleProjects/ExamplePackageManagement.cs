using Orc.NuGetExplorer.Models;

namespace Orc.NuGetExplorer.Management
{
    using NuGetExplorer.Models;
    using System;
    using System.Collections.Generic;

    public class ExamplePackageManagement : IExtensibleProject
    {
        public string Name => "Plain project extensible example";

        public ExamplePackageManagement(string rootPath)
        {
            ContentPath = System.IO.Path.Combine(rootPath, nameof(ExamplePackageManagement));
        }

        public IReadOnlyList<NuGetPackage> PackageList => throw new NotImplementedException();

        public string Framework => ".NETFramework,Version=v4.5";

        public string ContentPath { get; }

        public void Install()
        {
        }

        public void Uninstall()
        {
        }

        public void Update()
        {
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
