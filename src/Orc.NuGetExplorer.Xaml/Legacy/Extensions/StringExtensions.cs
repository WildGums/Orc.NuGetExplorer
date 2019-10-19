// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;
    using System.Windows.Media;

    public static class StringExtensions
    {
        #region Methods
        public static Inline ToInline(this string text)
        {
            return text.ToInline(Brushes.Black);
        }

        public static Inline ToInline(this string text, Brush brush)
        {
            var inline = new Run(text)
            {
                Foreground = brush ?? Brushes.Black
            };

            return inline;
        }
        #endregion
    }
}