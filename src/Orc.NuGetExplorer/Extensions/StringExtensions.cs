// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;
    using Catel;

    public static class StringExtensions
    {
        #region Methods
        public static string ToPackageSourceKey(this string packageSourceName)
        {
            Argument.IsNotNullOrWhitespace(() => packageSourceName);

            return string.Format("packageSource_{0}", packageSourceName);
        }

        public static Inline ToInline(this string text)
        {
            return new Run(text);
        }
        #endregion
    }
}