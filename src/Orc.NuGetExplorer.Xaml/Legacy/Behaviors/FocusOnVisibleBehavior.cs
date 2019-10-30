// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FocusOnVisibleBehavior.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using Catel.Windows.Interactivity;

    internal class FocusOnVisibleBehavior : BehaviorBase<FrameworkElement>
    {
        #region Methods
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

            AssociatedObject.Focus();
        }
        #endregion
    }
}