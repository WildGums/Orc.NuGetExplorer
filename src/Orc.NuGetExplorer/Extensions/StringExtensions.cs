// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using Catel;

    public static class StringExtensions
    {
        public static string ToPackageSourceKey(this string packageSourceName)
        {
            Argument.IsNotNullOrWhitespace(() => packageSourceName);

            return string.Format("packageSource_{0}", packageSourceName);
        }
    }
}