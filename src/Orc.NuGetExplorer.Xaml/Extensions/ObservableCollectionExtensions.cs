namespace Orc.NuGetExplorer;

using System;
using System.Collections.ObjectModel;

public static class ObservableCollectionExtensions
{
    public static void MoveUp<T>(this ObservableCollection<T> collection, T item)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(item);

        var oldIndex = collection.IndexOf(item);
        if (oldIndex == 0)
        {
            return;
        }

        collection.Move(oldIndex, oldIndex - 1);
    }

    public static void MoveDown<T>(this ObservableCollection<T> collection, T item)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(item);

        var oldIndex = collection.IndexOf(item);

        if (oldIndex == collection.Count - 1)
        {
            return;
        }

        collection.Move(oldIndex, oldIndex + 1);
    }
}
