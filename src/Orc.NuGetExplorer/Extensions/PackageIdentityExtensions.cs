namespace Orc.NuGetExplorer;

using System;
using NuGet.Packaging.Core;

public static class PackageIdentityExtensions
{
    public static string ToFullString(this PackageIdentity packageIdentity)
    {
        ArgumentNullException.ThrowIfNull(packageIdentity);

        return packageIdentity.HasVersion 
            ? $"{packageIdentity} {packageIdentity.Version.ToFullString()}"
            : packageIdentity.ToString();
    }
}
