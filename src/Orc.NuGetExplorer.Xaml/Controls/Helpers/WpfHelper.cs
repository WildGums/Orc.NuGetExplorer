namespace Orc.NuGetExplorer.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    public static class WpfHelper
    {
        public static TChild? FindVisualChild<TChild>(DependencyObject obj)
            where TChild : DependencyObject
        {
            ArgumentNullException.ThrowIfNull(obj);

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is TChild item)
                {
                    return item;
                }
                else
                {
                    var nestedChild = FindVisualChild<TChild>(child);

                    if (nestedChild is not null)
                    {
                        return nestedChild;
                    }
                }
            }
            return null;
        }
    }
}
