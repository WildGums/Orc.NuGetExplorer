// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------



namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static IList<string> SplitIfNonEmpty(this string value, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<string>();
            }

            return value.Split(separator);
        }

        public static bool ContainsAny(this string value, string[] str, StringComparison comparisonType)
        {
            for (int i = 0; i < str.Length; i++)
            {
                var s = str[i];
                if (value.Contains(s, comparisonType))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
