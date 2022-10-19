namespace Orc.NuGetExplorer
{
    using System;
    using System.Windows.Documents;
    using System.Windows.Media;

    public static class StringExtensions
    {
        public static Inline ToInline(this string text)
        {
            ArgumentNullException.ThrowIfNull(text);

            return text.ToInline(Brushes.Black);
        }

        public static Inline ToInline(this string text, Brush brush)
        {
            ArgumentNullException.ThrowIfNull(text);

            var inline = new Run(text)
            {
                Foreground = brush ?? Brushes.Black
            };

            return inline;
        }
    }
}
