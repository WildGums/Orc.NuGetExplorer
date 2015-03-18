// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageResolveService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Catel.Caching;

    internal class ImageResolveService : IImageResolveService
    {
        #region Fields
        private const string DefaultPackageUrl = "pack://application:,,,/Orc.NuGetExplorer;component/Resources/Images/packageDefaultIcon.png";
        private static readonly object Sync = new object();
        private readonly ICacheStorage<string, ImageSource> _packageDetailsCache = new CacheStorage<string, ImageSource>();
        #endregion

        #region Methods
        public ImageSource ResolveImageFromUri(Uri uri)
        {
            if (uri == null)
            {
                return GetDefaultImage();
            }

            lock (Sync)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uri.AbsoluteUri, () => CreateImage(uri));
            }
        }

        private ImageSource ResolveImageFromString(string uriString)
        {
            if (string.IsNullOrEmpty(uriString))
            {
                return GetDefaultImage();
            }

            lock (Sync)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uriString, () => CreateImage(new Uri(uriString)));
            }
        }

        private ImageSource CreateImage(Uri uri)
        {
            if (uri == null || (!string.Equals(DefaultPackageUrl, uri.AbsoluteUri) && !RemoteFileExists(uri.AbsoluteUri)))
            {
                return GetDefaultImage();
            }

            return new BitmapImage(uri);
        }

        private ImageSource GetDefaultImage()
        {
            return ResolveImageFromString(DefaultPackageUrl);
        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                var request = WebRequest.Create(url) as HttpWebRequest;

                request.Method = "GET";

                request.GetResponse();
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}