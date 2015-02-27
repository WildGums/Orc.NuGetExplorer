// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSourceOnVisibleBehavior.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Windows;
    using Catel.Windows.Interactivity;

    public class UpdateSourceOnVisibleBehavior : BehaviorBase<FrameworkElement>
    {
        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            AssociatedObject.IsVisibleChanged += OnIsVisibleChanged;

            base.OnAssociatedObjectLoaded();
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.IsVisibleChanged -= OnIsVisibleChanged;
            base.OnAssociatedObjectUnloaded();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool) e.NewValue)
            {
                return;
            }

            AssociatedObject.UpdateItemSource();
        }
        #endregion
    }
}