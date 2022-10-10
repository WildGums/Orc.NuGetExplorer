namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;

    public static class StringExtensions
    {
        public static string GetSafeScopeName(this string value)
        {
            if (value is null)
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
