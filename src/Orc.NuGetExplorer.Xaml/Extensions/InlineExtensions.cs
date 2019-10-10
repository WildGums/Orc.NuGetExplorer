// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InlineExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.old_NuGetExplorer
{
    using System.Collections.Generic;
    using System.Windows.Documents;

    public static class InlineExtensions
    {
        #region Methods
        public static Bold Bold(this Inline inline)
        {
            return new Bold(inline);
        }

        public static Inline Insert(this Inline inline, Inline inlineToAdd)
        {
            var span = inline as Span ?? new Span(inline);

            span.Inlines.Add(inlineToAdd);

            return span;
        }

        public static Inline Append(this Inline inline, Inline inlineToAdd)
        {
            var span = new Span(inline);

            span.Inlines.Add(inlineToAdd);

            return span;
        }

        public static Inline InsertRange(this Inline inline, IEnumerable<Inline> inlines)
        {
            var span = inline as Span ?? new Span(inline);

            span.Inlines.AddRange(inlines);

            return span;
        }

        public static Inline AppendRange(this Inline inline, IEnumerable<Inline> inlines)
        {
            var span = new Span(inline);

            span.Inlines.AddRange(inlines);

            return span;
        }
        #endregion
    }
}