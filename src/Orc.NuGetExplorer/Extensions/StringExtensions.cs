// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2016 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
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
    }
}