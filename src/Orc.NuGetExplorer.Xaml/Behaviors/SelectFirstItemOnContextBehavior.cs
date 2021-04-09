namespace Orc.NuGetExplorer.Behaviors
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Catel.Windows.Interactivity;

    internal class SelectFirstItemOnContextBehavior : BehaviorBase<ListBox>
    {
        protected override void OnAssociatedObjectLoaded()
        {
            TrySelectFirstItemFromSource();
        }

        private void TrySelectFirstItemFromSource()
        {
            if (AssociatedObject.Items is not null && AssociatedObject.Items.Count > 0)
            {
                AssociatedObject.SetCurrentValue(Selector.SelectedIndexProperty, 0);
            }
        }
    }
}
