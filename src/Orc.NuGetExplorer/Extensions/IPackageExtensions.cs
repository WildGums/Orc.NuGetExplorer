// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet;

    internal static class IPackageExtensions
    {
        public static bool IsPrerelease(this IPackage package)
        {
            Argument.IsNotNull(() => package);

            return !string.IsNullOrWhiteSpace(package.Version.SpecialVersion);
        }
    }
}