namespace Orc.NuGetExplorer
{
    using System;
    using NuGet.Packaging.Core;

    public static class PackageIdentityExtensions
    {
        public static string ToFullString(this PackageIdentity packageIdentity)
        {
            ArgumentNullException.ThrowIfNull(packageIdentity);

            if (packageIdentity.HasVersion)
            {
                return $"{packageIdentity} {packageIdentity.Version.ToFullString()}";
            }

            return packageIdentity.ToString();
        }
    }
}
