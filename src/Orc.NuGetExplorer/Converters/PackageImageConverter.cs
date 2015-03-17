// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageImageConverter.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Windows.Media.Imaging;
    using Catel.MVVM.Converters;

    public class PackageImageConverter : ValueConverterBase
    {
        #region Methods
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var uri = value as Uri;
            if (uri == null)
            {
                uri = new Uri("http://nuget.org/Content/Images/packageDefaultIcon.png");
            }

            return new BitmapImage(uri);
        }
        #endregion
    }
}