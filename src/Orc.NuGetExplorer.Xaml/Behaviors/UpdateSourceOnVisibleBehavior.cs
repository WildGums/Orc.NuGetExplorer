﻿namespace Orc.NuGetExplorer.Behaviors;

using System.Windows;
using Catel.Windows.Interactivity;

internal class UpdateSourceOnVisibleBehavior : BehaviorBase<FrameworkElement>
{
    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        AssociatedObject.IsVisibleChanged += OnIsVisibleChanged;
    }

    protected override void OnAssociatedObjectUnloaded()
    {
        AssociatedObject.IsVisibleChanged -= OnIsVisibleChanged;

        base.OnAssociatedObjectUnloaded();
    }

    private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(bool)e.NewValue)
        {
            return;
        }

        AssociatedObject.UpdateItemSource();
    }
}