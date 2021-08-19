namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Catel.Logging;

    public static class WebClientExtensions
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public async static Task<byte[]> LogAndDownloadDataTaskAsync(this WebClient client, Uri uri)
        {
            Log.Debug($"Webclient request on {uri}");

            var array = await client.DownloadDataTaskAsync(uri);

            return array;
        }

        public static byte[] LogAndDownloadData(this WebClient client, Uri uri)
        {
            Log.Debug($"Webclient request on {uri}");

            var array = client.DownloadData(uri);

            return array;
        }
    }
}
