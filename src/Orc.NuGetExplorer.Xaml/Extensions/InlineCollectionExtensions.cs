namespace Orc.NuGetExplorer
{
    using System;
    using System.Windows.Documents;

    public static class InlineCollectionExtensions
    {
        public static void AddIfNotNull(this InlineCollection inlineCollection, Inline inline)
        {
            ArgumentNullException.ThrowIfNull(inlineCollection);

            if (inline is null)
            {
                return;
            }

            inlineCollection.Add(inline);
        }
    }
}
