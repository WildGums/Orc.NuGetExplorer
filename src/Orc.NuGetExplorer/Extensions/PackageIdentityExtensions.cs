namespace Orc.NuGetExplorer
{
    using NuGet.Packaging.Core;

    public static class PackageIdentityExtensions
    {
        public static string ToFullString(this PackageIdentity packageIdentity)
        {
            if(packageIdentity.HasVersion)
            {
                return $"{packageIdentity} {packageIdentity.Version.ToFullString()}";
            }

            return packageIdentity.ToString();
        }
    }
}
