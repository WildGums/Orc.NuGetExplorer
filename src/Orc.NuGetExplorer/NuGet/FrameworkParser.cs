namespace Orc.NuGetExplorer
{
    using System;
    using Catel.Logging;
    using NuGet.Frameworks;

    public static class FrameworkParser
    {
        public static NuGetFramework TryParseFrameworkName(string frameworkString, IFrameworkNameProvider frameworkNameProvider)
        {
            try
            {
                return NuGetFramework.ParseFrameworkName(frameworkString, frameworkNameProvider);
            }
            catch (ArgumentException)
            {
                return NuGetFramework.UnsupportedFramework;
            }
        }
    }
}
