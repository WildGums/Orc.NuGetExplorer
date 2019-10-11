namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public static class ObjectExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
