// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSelectedItemSourceBehavior.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Catel.Windows.Interactivity;

    public class UpdateSelectedItemSourceBehavior : BehaviorBase<Selector>
    {
        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.SelectionChanged += OnIsVisibleChanged;

            base.OnAssociatedObjectLoaded();
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.SelectionChanged -= OnIsVisibleChanged;
            base.OnAssociatedObjectUnloaded();
        }

        private void OnIsVisibleChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var binding = AssociatedObject.GetBindingExpression(Selector.SelectedItemProperty);
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
        #endregion
    }
}