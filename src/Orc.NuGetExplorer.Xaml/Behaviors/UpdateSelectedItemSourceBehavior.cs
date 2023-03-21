namespace Orc.NuGetExplorer.Behaviors;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Catel.Windows.Interactivity;

internal class UpdateSelectedItemSourceBehavior : BehaviorBase<Selector>
{
    protected override void OnAssociatedObjectLoaded()
    {
        base.OnAssociatedObjectLoaded();

        AssociatedObject.SelectionChanged += OnIsVisibleChanged;
    }

    protected override void OnAssociatedObjectUnloaded()
    {
        AssociatedObject.SelectionChanged -= OnIsVisibleChanged;

        base.OnAssociatedObjectUnloaded();
    }

    private void OnIsVisibleChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
    {
        var binding = AssociatedObject.GetBindingExpression(Selector.SelectedItemProperty);
        if (binding is not null)
        {
            binding.UpdateSource();
        }
    }
}