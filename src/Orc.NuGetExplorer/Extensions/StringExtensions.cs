// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="Orchestra development team">
//   Copyright (c) 2008 - 2014 Orchestra development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;

    public static class StringExtensions
    {
        public static bool IsAllSource(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            return string.Equals("all", value, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}