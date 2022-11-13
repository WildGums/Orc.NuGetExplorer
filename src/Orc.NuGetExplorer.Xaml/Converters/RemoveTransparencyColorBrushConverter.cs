namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Windows.Media;
    using Catel.MVVM.Converters;
    using Orc.Theming;

    [System.Windows.Data.ValueConversion(typeof(Color), typeof(SolidColorBrush))]
    public class RemoveTransparencyColorBrushConverter : ValueConverterBase<Color, SolidColorBrush>
    {
        private SolidColorBrush? _cachedBrush;

        protected override object Convert(Color value, Type targetType, object? parameter)
        {
            if (_cachedBrush is not null)
            {
                return _cachedBrush;
            }

            var nonTransparentColor = AccentColorHelper.ConvertToNonAlphaColor(Colors.White, value);
            var nonTransparentBrush = nonTransparentColor.ToSolidColorBrush();

            nonTransparentBrush.Freeze();
            _cachedBrush = nonTransparentBrush;

            return nonTransparentBrush;
        }
    }
}
