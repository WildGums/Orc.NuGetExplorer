namespace Orc.NuGetExplorer;

using System;
using System.Text.RegularExpressions;
using Catel;
using Catel.Logging;
using NuGet.Packaging.Core;
using NuGet.Versioning;

public class PackageIdentityParser
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    /// <summary>
    /// This regex follows the same rules as C# namespaces
    /// </summary>
    private static readonly string IdentityPattern = @"(@?[a-z_A-Z]\w+(?:\.@?[a-z_A-Z]\w+)*)";

    public static PackageIdentity? Parse(string packageString)
    {
        Argument.IsNotNullOrEmpty(() => packageString);

        var rgx = new Regex(IdentityPattern, RegexOptions.None, TimeSpan.FromSeconds(1));

        var match = rgx.Match(packageString);

        if (!match.Success)
        {
            Log.Warning($"{packageString} {Constants.Messages.PackageParserInvalidIdentity}");
            return null;
        }

        if (match.Captures.Count != 1)
        {
            Log.Warning($"{packageString} {Constants.Messages.PackageParserInvalidIdentity}");
            return null;
        }

        var identity = match.Captures[0].Value;

        var versionString = packageString.Replace(identity, string.Empty);

        if (!NuGetVersion.TryParse(versionString.TrimStart('.'), out var version))
        {
            Log.Warning($"{packageString} {Constants.Messages.PackageParserInvalidVersion}");
            return null;
        }

        return new PackageIdentity(identity, version);
    }
}