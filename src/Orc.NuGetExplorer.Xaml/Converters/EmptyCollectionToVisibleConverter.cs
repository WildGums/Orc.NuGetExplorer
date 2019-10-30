namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Data;
    using Catel.Logging;
    using Catel.MVVM.Converters;

    [ValueConversion(typeof(ICollection), typeof(Visibility))]
    public class EmptyCollectionToVisibleConverter : ValueConverterBase<ICollection, Visibility>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly CollectionToCollapsingVisibilityConverter CollectionToVisibility = new CollectionToCollapsingVisibilityConverter();

        protected override object Convert(ICollection value, Type targetType, object parameter)
        {
            try
            {
                var visibility = (Visibility)CollectionToVisibility.Convert(value, targetType, parameter, CurrentCulture);

                return visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception e)
            {
                Log.Error("Error occured during value conversion", e);
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
