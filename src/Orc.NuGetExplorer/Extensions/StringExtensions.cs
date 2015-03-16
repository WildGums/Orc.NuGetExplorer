// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;
    using Catel;

    internal static class StringExtensions
    {
        #region Methods
        public static string ToPackageSourceKey(this string packageSourceName)
        {
            Argument.IsNotNullOrWhitespace(() => packageSourceName);

            var value = string.Format("packageSource_{0}", packageSourceName);

            // Replace special characters
            value = value.Replace(":", "_");
            value = value.Replace(" ", "_");

            return value;
        }

        public static Inline ToInline(this string text)
        {
            return new Run(text);
        }
        #endregion
    }
}