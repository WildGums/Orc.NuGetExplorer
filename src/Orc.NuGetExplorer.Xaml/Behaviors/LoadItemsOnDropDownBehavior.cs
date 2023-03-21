﻿namespace Orc.NuGetExplorer.Behaviors;

using System;
using System.Windows;
using System.Windows.Controls;
using Catel.MVVM;

internal class LoadItemsOnDropDownBehavior : Catel.Windows.Interactivity.BehaviorBase<ComboBox>
{
    protected override void OnAssociatedObjectLoaded()
    {
        AssociatedObject.DropDownOpened += OnAssociatedObjectDropDownOpened;
    }

    private void OnAssociatedObjectDropDownOpened(object? sender, EventArgs e)
    {
        ExecuteItemSourceInitializationCommand();
    }

    public Command? Command
    {
        get { return (Command?)GetValue(CommandProperty); }
        set { SetValue(CommandProperty, value); }
    }

    /// <summary>
    /// Identifies the <see cref="Command"/> 
    /// dependency property.</summary>
    public static readonly DependencyProperty CommandProperty =
        DependencyProperty.Register(nameof(Command), typeof(Command), typeof(LoadItemsOnDropDownBehavior), new PropertyMetadata(null));

    private void ExecuteItemSourceInitializationCommand()
    {
        using (SynchronizationContextScopeManager.OutOfContext())
        {
            ExecuteCommand();
        }
    }

    private void ExecuteCommand()
    {
        Command?.Execute();
    }
}