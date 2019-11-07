namespace Orc.NuGetExplorer
{
    using System;
    using System.Text.RegularExpressions;
    using NuGet.Packaging.Core;
    using NuGet.Versioning;

    public class PackageIdentityParser
    {
        /// <summary>
        /// This regex follows the same rules as C# namespaces
        /// </summary>
        private static readonly string IdentityPattern = @"(@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*)";

        public static PackageIdentity Parse(string packageString)
        {
            var rgx = new Regex(IdentityPattern);

            var match = rgx.Match(packageString);

            if (!match.Success)
            {
                throw new ArgumentException($"{packageString} parameter doesn't contain valid package identity");
            }

            if (match.Captures.Count != 1)
            {
                throw new ArgumentException($"{packageString} parameter doesn't contain valid package identity");
            }

            var identity = match.Captures[0].Value;

            var versionString = packageString.Replace(identity, "");

            if (!NuGetVersion.TryParse(versionString.TrimStart('.'), out NuGetVersion version))
            {
                throw new ArgumentException($"{packageString} parameter doesn't contain valid package version");
            }

            return new PackageIdentity(identity, version);
        }
    }
}
