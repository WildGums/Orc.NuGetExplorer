// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static string GetSafeScopeName(this string value)
        {
            if (value == null)
            {
                return "null";
            }

            return value.ToLower();
        }

        public static IList<string> SplitOrEmpty(this string value, char separator = ',')
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                return value.Split(separator);
            }

            return new List<string>();
        }
    }
}
