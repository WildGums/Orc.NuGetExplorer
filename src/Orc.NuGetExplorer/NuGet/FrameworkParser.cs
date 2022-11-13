namespace Orc.NuGetExplorer
{
    using System;
    using System.Runtime.InteropServices;
    using Catel;
    using NuGet.Frameworks;

    public static class FrameworkParser
    {
        public static NuGetFramework TryParseFrameworkName(string frameworkString, IFrameworkNameProvider frameworkNameProvider)
        {
            Argument.IsNotNullOrEmpty(() => frameworkString);
            ArgumentNullException.ThrowIfNull(frameworkNameProvider);

            try
            {
                return NuGetFramework.ParseFrameworkName(frameworkString, frameworkNameProvider);
            }
            catch (ArgumentException)
            {
                return NuGetFramework.UnsupportedFramework;
            }
        }

        /// <summary>
        /// Returns TFM related to specific platform. This works only for net 5.0 and above
        /// </summary>
        /// <returns></returns>
        public static NuGetFramework ToSpecificPlatform(NuGetFramework framework)
        {
            ArgumentNullException.ThrowIfNull(framework);

            if (framework.Version.Major < 5)
            {
                return framework;
            }

#if __ANDROID__
            return new NuGetFramework(framework.Framework, framework.Version, "android", new Version());
#elif __IOS__
            return new NuGetFramework(framework.Framework, framework.Version, "ios", new Version());
#endif
            // Try to determine platform OS
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // TFM's versions does not match to OS versions, we can just use last to match all actual frameworks
                return new NuGetFramework(framework.Framework, framework.Version, "windows", new Version(10, 0, 0));
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return new NuGetFramework(framework.Framework, framework.Version, "macos", Environment.OSVersion.Version);
            }

            return framework;
        }
    }
}
