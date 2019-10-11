namespace Orc.NuGetExplorer.Behaviors
{
    using Catel.Windows.Interactivity;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    public class SelectFirstItemOnContextBehavior : BehaviorBase<ListBox>
    {
        protected override void OnAssociatedObjectLoaded()
        {
            TrySelectFirstItemFromSource();
        }

        private void TrySelectFirstItemFromSource()
        {
            if (AssociatedObject.Items != null && AssociatedObject.Items.Count > 0)
            {
                AssociatedObject.SetCurrentValue(Selector.SelectedIndexProperty, 0);
            }
        }
    }
}
