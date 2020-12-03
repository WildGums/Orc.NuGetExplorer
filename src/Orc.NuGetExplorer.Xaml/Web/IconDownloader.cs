namespace Orc.NuGetExplorer.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Catel.Logging;

    public class IconDownloader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IconDownloader()
        {
            SetProtocolSecurity();
        }

        public async Task<byte[]> GetByUrlAsync(Uri uri, WebClient client)
        {
            Log.Debug($"Webclient request on {uri}");

            var array = await client.DownloadDataTaskAsync(uri);

            return array;
        }

        public byte[] GetByUrl(Uri uri, WebClient client)
        {
            Log.Debug($"Webclient request on {uri}");

            var array = client.DownloadData(uri);

            return array;
        }

        private static void SetProtocolSecurity()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls | SecurityProtocolType.Tls13;
        }
    }
}
