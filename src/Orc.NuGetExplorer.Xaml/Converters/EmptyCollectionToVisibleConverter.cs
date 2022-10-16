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

        private static readonly CollectionToCollapsingVisibilityConverter CollectionToVisibility = new();

        protected override object? Convert(ICollection? value, Type targetType, object? parameter)
        {
            try
            {
                var visibilityBoxed = CollectionToVisibility.Convert(value, targetType, parameter, CurrentCulture);
                if (visibilityBoxed is null)
                {
                    return Visibility.Visible;
                }

                return (Visibility)visibilityBoxed == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

            }
            catch (Exception ex)
            {
                Log.Error("Error occured during value conversion", ex);
                return DependencyProperty.UnsetValue;
            }
        }
    }
}
