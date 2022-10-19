namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Documents;

    public static class InlineExtensions
    {
        public static Bold Bold(this Inline inline)
        {
            return new Bold(inline);
        }

        public static Inline Insert(this Inline inline, Inline inlineToAdd)
        {
            ArgumentNullException.ThrowIfNull(inline);

            var span = inline as Span ?? new Span(inline);

            span.Inlines.Add(inlineToAdd);

            return span;
        }

        public static Inline Append(this Inline inline, Inline inlineToAdd)
        {
            ArgumentNullException.ThrowIfNull(inline);

            var span = new Span(inline);

            span.Inlines.Add(inlineToAdd);

            return span;
        }

        public static Inline InsertRange(this Inline inline, IEnumerable<Inline> inlines)
        {
            ArgumentNullException.ThrowIfNull(inline);

            var span = inline as Span ?? new Span(inline);

            span.Inlines.AddRange(inlines);

            return span;
        }

        public static Inline AppendRange(this Inline inline, IEnumerable<Inline> inlines)
        {
            ArgumentNullException.ThrowIfNull(inline);

            var span = new Span(inline);

            span.Inlines.AddRange(inlines);

            return span;
        }
    }
}
