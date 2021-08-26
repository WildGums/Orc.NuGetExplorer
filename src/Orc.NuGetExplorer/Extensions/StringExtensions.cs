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
        public static IList<string> SplitIfNonEmpty(this string value, char separator = ',')
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new List<string>();
            }

            return value.Split(separator);
        }
    }
}
