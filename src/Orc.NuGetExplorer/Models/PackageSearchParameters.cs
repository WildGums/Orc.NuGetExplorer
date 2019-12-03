namespace Orc.NuGetExplorer.Models
{
    public class PackageSearchParameters
    {
        public PackageSearchParameters(bool prereleasIncluded, string searchString)
        {
            SearchString = searchString;
            IsPrereleaseIncluded = prereleasIncluded;
        }

        public PackageSearchParameters()
        {
            SearchString = string.Empty;
            IsPrereleaseIncluded = false;
        }

        public bool IsPrereleaseIncluded { get; set; }

        public string SearchString { get; set; }
    }
}
