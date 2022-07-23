namespace Orc.NuGetExplorer
{
    using System;
    using Catel;
    using NuGet.Frameworks;

    public static class NuGetFrameworkExtensions
    {
        public static bool IsNet5Era(this NuGetFramework nuGetFramework)
        {
            Argument.IsNotNull(() => nuGetFramework);

            return (nuGetFramework.Version.Major >= 5 && StringComparer.OrdinalIgnoreCase.Equals(FrameworkConstants.FrameworkIdentifiers.NetCoreApp, nuGetFramework.Framework));
        }
    }
}
