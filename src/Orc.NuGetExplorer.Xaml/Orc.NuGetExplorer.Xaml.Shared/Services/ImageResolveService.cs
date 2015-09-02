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
        private readonly object _lockObject = new object();
        private readonly ICacheStorage<string, ImageSource> _packageDetailsCache = new CacheStorage<string, ImageSource>();
        #endregion

        #region Methods
        public ImageSource ResolveImageFromUri(Uri uri, string defaultUrl = null)
        {
            if (uri == null)
            {
                return GetDefaultImage(defaultUrl);
            }

            lock (_lockObject)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uri.AbsoluteUri, () => CreateImage(uri, defaultUrl));
            }
        }

        private ImageSource ResolveImageFromString(string uriString, string defaultUrl)
        {
            if (string.IsNullOrEmpty(uriString))
            {
                return GetDefaultImage(defaultUrl);
            }

            lock (_lockObject)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uriString, () => CreateImage(new Uri(uriString), defaultUrl));
            }
        }

        private ImageSource CreateImage(Uri uri, string defaultUrl)
        {
            if (uri == null || (!string.Equals(defaultUrl, uri.AbsoluteUri) && !RemoteFileExists(uri.AbsoluteUri)))
            {
                return GetDefaultImage(defaultUrl);
            }

            return new BitmapImage(uri);
        }

        private ImageSource GetDefaultImage(string defaultUrl)
        {
            if (string.IsNullOrEmpty(defaultUrl))
            {
                return null;
            }
            return ResolveImageFromString(defaultUrl, defaultUrl);
        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(url);

                request.Method = "GET";
                request.Timeout = 5000;

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