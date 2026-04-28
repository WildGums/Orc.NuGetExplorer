namespace Orc.NuGetExplorer.Resolver;

using System.Collections.Generic;
using System.Threading;
using NuGet.Packaging.Core;

public static class PackageResolverExtensions
{
    public static IReadOnlyList<PackageIdentity> Resolve(this PackageResolver resolver, PackageResolverContext context, CancellationToken token)
    {
        return resolver.Resolve(context, token);
    }
}
