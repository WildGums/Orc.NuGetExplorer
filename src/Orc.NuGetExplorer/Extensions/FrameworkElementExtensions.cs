// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameworkElementExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Media;
    using Catel;

    public static class FrameworkElementExtensions
    {
        #region Methods
        public static void UpdateItemSource(this FrameworkElement frameworkElement)
        {
            Argument.IsNotNull(() => frameworkElement);

            var infos = frameworkElement.GetType().GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Static);

            foreach (var field in infos.Where(x => x.FieldType == typeof (DependencyProperty)))
            {
                var dp = (DependencyProperty) field.GetValue(null);
                var bindingExpression = frameworkElement.GetBindingExpression(dp);
                if (bindingExpression == null)
                {
                    continue;
                }

                if (bindingExpression.ParentBinding.UpdateSourceTrigger == UpdateSourceTrigger.Explicit)
                {
                    bindingExpression.UpdateSource();
                }
            }

            var count = VisualTreeHelper.GetChildrenCount(frameworkElement);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(frameworkElement, i) as FrameworkElement;
                if (child != null)
                {
                    child.UpdateItemSource();
                }
            }
        }
        #endregion
    }
}