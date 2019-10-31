namespace Orc.NuGetExplorer.Management
{
    using System;
    using System.Collections.Generic;
    using Orc.NuGetExplorer.Models;

    /// <summary>
    /// Default project which represents "plugins" folder
    /// </summary>
    public class DestFolder : IExtensibleProject
    {
        public DestFolder(string destinationFolder)
        {
            ContentPath = destinationFolder;
        }

        public string Name => "Plugins";

        public string Framework => ".NETStandard,Version=v2.1";

        public string ContentPath { get; private set; }


        public IReadOnlyList<NuGetPackage> PackageList => throw new NotImplementedException();

        public void Install()
        {
            throw new NotImplementedException();
        }

        public void Uninstall()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
