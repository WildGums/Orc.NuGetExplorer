// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSelectedItemSourceBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Catel.Windows.Interactivity;

    internal class UpdateSelectedItemSourceBehavior : BehaviorBase<Selector>
    {
        #region Methods
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
            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
        #endregion
    }
}