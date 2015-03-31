// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageResolveService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Runtime.InteropServices;
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
        public ImageSource ResolveImageFromUri(Uri uri)
        {
            if (uri == null)
            {
                return GetDefaultImage();
            }

            lock (_lockObject)
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

            lock (_lockObject)
            {
                return _packageDetailsCache.GetFromCacheOrFetch(uriString, () => CreateImage(new Uri(uriString)));
            }
        }

        private ImageSource CreateImage(Uri uri)
        {
            if (uri == null ||  !RemoteFileExists(uri.AbsoluteUri))
            {
                return GetDefaultImage();
            }

            return new BitmapImage(uri);
        }

        private BitmapImage _defaultImage;

        private ImageSource GetDefaultImage()
        {
            if (_defaultImage != null)
            {
                return _defaultImage;
            }

            var assembly = GetType().Assembly;
            var bitmapImage = new BitmapImage();
            var manifestResourceName = assembly.GetManifestResourceNames().FirstOrDefault(x => x.Contains("packageDefaultIcon.png"));
            if (string.IsNullOrEmpty(manifestResourceName))
            {
                return bitmapImage;
            }
            using (var stream = assembly.GetManifestResourceStream(manifestResourceName))
            {
                if (stream != null)
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = stream;
                    bitmapImage.EndInit();
                }
            }
            _defaultImage = bitmapImage;

            return _defaultImage;
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