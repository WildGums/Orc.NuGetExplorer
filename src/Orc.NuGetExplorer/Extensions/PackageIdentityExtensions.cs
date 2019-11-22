namespace Orc.NuGetExplorer
{
    using NuGet.Packaging.Core;

    public static class PackageIdentityExtensions
    {
        //TODO check is needed or standart implementation is enough
        public static string ToFullString(this PackageIdentity packageIdentity)
        {
            return $"{packageIdentity} {packageIdentity.Version.ToFullString()}";
        }
    }
}
