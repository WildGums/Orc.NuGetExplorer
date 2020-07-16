namespace Orc.NuGetExplorer.Resolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using NuGet.Packaging.Core;

    public static class PackageResolverExtensions
    {
        public static IEnumerable<PackageIdentity> Resolve(this PackageResolver resolver, PackageResolverContext context, CancellationToken token)
        {
            return resolver.Resolve(context, token);
        }
    }
}
