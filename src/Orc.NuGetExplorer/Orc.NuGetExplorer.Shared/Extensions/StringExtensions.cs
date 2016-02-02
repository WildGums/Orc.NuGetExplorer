// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2016 Wild Gums. All rights reserved.
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