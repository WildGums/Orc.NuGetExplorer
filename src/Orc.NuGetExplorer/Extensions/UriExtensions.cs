namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel.Logging;

    public static class UriExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public static Uri GetRootUri(this Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            return new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
        }

        public static Uri? GetLocalUriForFragment(this Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            if (!string.IsNullOrEmpty(uri.Fragment))
            {
                var fileName = uri.Fragment[1..];
                var folderPath = Path.GetDirectoryName(uri.LocalPath);
                if (string.IsNullOrEmpty(folderPath))
                {
                    Log.Debug("Cannot parse source uri for local icon");
                    return null;
                }

                return new Uri(Path.Combine(folderPath, fileName));
            }

            return null;
        }
    }
}
