namespace Orc.NuGetExplorer;

public class PackageSearchParameters
{
    public PackageSearchParameters(bool prereleaseIncluded, string searchString, bool isRecommendedOnly)
    {
        SearchString = searchString;
        IsPrereleaseIncluded = prereleaseIncluded;
        IsRecommendedOnly = isRecommendedOnly;
    }

    public PackageSearchParameters()
    {
        SearchString = string.Empty;
        IsPrereleaseIncluded = false;
        IsRecommendedOnly = false;
    }

    public bool IsPrereleaseIncluded { get; set; }

    public string SearchString { get; set; }

    /// <summary>
    /// Option for updates
    /// </summary>
    public bool IsRecommendedOnly { get; set; }
}
