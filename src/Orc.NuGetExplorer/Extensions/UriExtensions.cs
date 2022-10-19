namespace Orc.NuGetExplorer
{
    using System;

    public static class UriExtensions
    {
        public static Uri GetRootUri(this Uri uri)
        {
            ArgumentNullException.ThrowIfNull(uri);

            return new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped));
        }
    }
}
