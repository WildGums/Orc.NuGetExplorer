namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Windows.Media;
    using Catel.MVVM.Converters;
    using NuGetExplorer.Enums;

    [System.Windows.Data.ValueConversion(typeof(PackageStatus), typeof(Brush))]
    public class PackageStatusEnumToBrushConverter : ValueConverterBase<PackageStatus, Brush>
    {
        private static readonly int Offset = -1;
        private readonly Themes.Brushes _resourceDictionary;

        public PackageStatusEnumToBrushConverter()
        {
            _resourceDictionary = new Themes.Brushes();

            _resourceDictionary.InitializeComponent();
        }

        protected override object Convert(PackageStatus value, Type targetType, object parameter)
        {
            var resourceKeys = parameter as string[];

            if (resourceKeys is null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            int keyIndex = (int)value - Offset;

            if (keyIndex >= 0 && keyIndex < resourceKeys.Length)
            {
                var brushResource = _resourceDictionary[resourceKeys[keyIndex]];

                return brushResource;
            }

            return new SolidColorBrush(Colors.Transparent);
        }
    }
}
