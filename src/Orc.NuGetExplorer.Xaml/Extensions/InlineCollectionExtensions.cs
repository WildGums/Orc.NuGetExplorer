// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InlineCollectionExtensions.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using System.Windows.Documents;

    public static class InlineCollectionExtensions
    {
        public static void AddIfNotNull(this InlineCollection inlineCollection, Inline inline)
        {
            if (inline == null)
            {
                return;
            }

            inlineCollection.Add(inline);
        }
    }
}