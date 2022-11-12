namespace Orc.NuGetExplorer.Example
{
    using System.Collections.Generic;

    public class DefaultPackageSourcesProvider : IDefaultPackageSourcesProvider
    {
        public string DefaultSource { get; set; } = Constants.DefaultNuGetOrgUri;

        public IEnumerable<IPackageSource> GetDefaultPackages()
        {
            return new List<IPackageSource>
            {
                new NuGetFeed("nuget.org", "https://api.nuget.org/v3/index.json"),
                new NuGetFeed("Microsoft Visual Studio Offline Packages", @"C:\Program Files (x86)\Microsoft SDKs\NuGetPackages\")
            };
        }
    }
}
