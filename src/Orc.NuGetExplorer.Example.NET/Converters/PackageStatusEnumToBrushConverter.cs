using Orc.NuGetExplorer.Enums;

namespace Orc.NuGetExplorer.Converters
{
    using Catel.MVVM.Converters;
    using NuGetExplorer.Enums;
    using System;
    using System.Windows.Media;

    public class PackageStatusEnumToBrushConverter : ValueConverterBase<PackageStatus, Brush>
    {
        int offset = -1;

        readonly Themes.Brushes resourceDictionary;

        public PackageStatusEnumToBrushConverter()
        {
            resourceDictionary = new Themes.Brushes();

            resourceDictionary.InitializeComponent();
        }

        protected override object Convert(PackageStatus value, Type targetType, object parameter)
        {
            var resourceKeys = parameter as string[];

            if (resourceKeys == null)
            {
                return new SolidColorBrush(Colors.Transparent);
            }

            int keyIndex = (int)value - offset;

            if (keyIndex >= 0 && keyIndex < resourceKeys.Length)
            {
                var brushResource = resourceDictionary[resourceKeys[keyIndex]];

                return brushResource;
            }

            return new SolidColorBrush(Colors.Transparent);
        }
    }
}
