// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageImageConverter.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Converters
{
    using System;
    using System.Windows.Media.Imaging;
    using Catel.Caching;
    using Catel.MVVM.Converters;

    public class PackageImageConverter : ValueConverterBase
    {
        #region Fields
        private static readonly object Sync = new object();
        private readonly ICacheStorage<string, BitmapImage> _packageDetailsCache = new CacheStorage<string, BitmapImage>();
        #endregion

        #region Methods
        protected override object Convert(object value, Type targetType, object parameter)
        {
            var uri = value as Uri;
            if (uri == null)
            {
                return ResolveImageFromString("http://nuget.org/Content/Images/packageDefaultIcon.png");
            }

            return ResolveImageFromUri(uri);
        }

        private BitmapImage ResolveImageFromString(string uriString)
        {
            lock (Sync)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uriString, () => new BitmapImage(new Uri(uriString)));
            }
        }

        private BitmapImage ResolveImageFromUri(Uri uri)
        {
            lock (Sync)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uri.AbsoluteUri, () => new BitmapImage(uri));
            }
        }
        #endregion
    }
}