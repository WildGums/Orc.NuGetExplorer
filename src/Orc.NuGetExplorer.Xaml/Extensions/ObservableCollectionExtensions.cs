﻿namespace Orc.NuGetExplorer;

using System;
using System.Collections.ObjectModel;

public static class ObservableCollectionExtensions
{
    public static void MoveUp<T>(this ObservableCollection<T> collection, T item)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(item);

        var oldindex = collection.IndexOf(item);
        if (oldindex == 0)
        {
            return;
        }

        collection.Move(oldindex, oldindex - 1);
    }

    public static void MoveDown<T>(this ObservableCollection<T> collection, T item)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(collection);
        ArgumentNullException.ThrowIfNull(item);

        var oldindex = collection.IndexOf(item);

        if (oldindex == collection.Count - 1)
        {
            return;
        }

        collection.Move(oldindex, oldindex + 1);
    }
}