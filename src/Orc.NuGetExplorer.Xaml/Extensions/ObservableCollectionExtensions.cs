namespace Orc.NuGetExplorer
{
    using System.Collections.ObjectModel;
    using Catel;

    public static class ObservableCollectionExtensions
    {
        public static void MoveUp<T>(this ObservableCollection<T> collection, T item)
        {
            Argument.IsNotNull(() => collection);
            Argument.IsNotNull(nameof(item), item);

            var oldindex = collection.IndexOf(item);
            if (oldindex == 0)
            {
                return;
            }

            collection.Move(oldindex, oldindex - 1);
        }

        public static void MoveDown<T>(this ObservableCollection<T> collection, T item)
        {
            Argument.IsNotNull(() => collection);
            Argument.IsNotNull(nameof(item), item);

            var oldindex = collection.IndexOf(item);

            if (oldindex == collection.Count - 1)
            {
                return;
            }

            collection.Move(oldindex, oldindex + 1);
        }
    }
}
