namespace Orc.NuGetExplorer.Models
{
    public class PackageSearchParameters
    {
        public PackageSearchParameters(bool prereleasIncluded, string searchString)
        {
            SearchString = searchString;
            IsPrereleaseIncluded = prereleasIncluded;
        }

        public bool IsPrereleaseIncluded { get; set; }

        public string SearchString { get; set; }
    }
}
