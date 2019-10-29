using System;
using Catel.Logging;
using NuGet.Frameworks;

namespace Orc.NuGetExplorer
{
    public static class FrameworkParser
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static NuGetFramework TryParseFrameworkName(string frameworkString, IFrameworkNameProvider frameworkNameProvider)
        {
            try
            {
                return NuGetFramework.ParseFrameworkName(frameworkString, frameworkNameProvider);
            }
            catch (ArgumentException e)
            {
                Log.Error(e, "Incorrect target framework");
                throw;
            }
        }
    }
}
