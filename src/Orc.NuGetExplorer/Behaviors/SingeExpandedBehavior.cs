// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingeExpandedBehavior.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using Catel;
    using Catel.Windows.Interactivity;

    internal class SingeExpandedBehavior : BehaviorBase<Expander>
    {
        #region Fields
        private static readonly IList<Expander> _associatedExpanders = new BindingList<Expander>();
        private static bool _expanding;
        #endregion

        #region Methods
        protected override void OnAssociatedObjectLoaded()
        {
            _associatedExpanders.Add(AssociatedObject);
            AssociatedObject.Expanded += OnExpanded;
            AssociatedObject.Collapsed += OnExpanded;
            base.OnAssociatedObjectLoaded();
        }

        protected override void OnAssociatedObjectUnloaded()
        {
            AssociatedObject.Expanded -= OnExpanded;
            AssociatedObject.Collapsed -= OnExpanded;
            _associatedExpanders.Remove(AssociatedObject);
            base.OnAssociatedObjectUnloaded();
        }

        private void OnExpanded(object sender, RoutedEventArgs routedEventArgs)
        {
            if (_expanding)
            {
                return;
            }

            using (new DisposableToken(this, token => _expanding = true, token => _expanding = false))
            {
                AssociatedObject.IsExpanded = true;
                foreach (var expander in _associatedExpanders.Where(x => !Equals(x, AssociatedObject)))
                {
                    expander.IsExpanded = false;
                }
            }
        }
        #endregion
    }
}