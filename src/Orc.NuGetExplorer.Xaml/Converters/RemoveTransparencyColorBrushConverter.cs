namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Catel.MVVM.Converters;
    using Orc.Theming;

    public class RemoveTransparencyColorBrushConverter : ValueConverterBase<Color, SolidColorBrush>
    {
        private SolidColorBrush _cachedBrush;

        protected override object Convert(Color value, Type targetType, object parameter)
        {
            if (_cachedBrush != null)
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
