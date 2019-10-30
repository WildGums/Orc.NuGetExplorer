namespace Orc.NuGetExplorer.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using System.Windows.Media;
    using Catel.Logging;

    public class IconDownloader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        public IconDownloader()
        {
            //this can be danger and considered harmful (we trust to all cerfs)
            //more info https://stackoverflow.com/questions/703272/could-not-establish-trust-relationship-for-ssl-tls-secure-channel-soap/6613434#6613434
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<byte[]> GetByUrlAsync(Uri uri, WebClient client)
        {
            Log.Debug($"Begin webclient request on {uri}");

            //while (client.IsBusy)
            //{
            //    Log.Debug("Waiting for ending current download");
            //    await Task.Delay(200);
            //}
            var array = await client.DownloadDataTaskAsync(uri);

            Log.Debug($"Webclient request ended");

            return array;
        }

        public byte[] GetByUrl(Uri uri, WebClient client)
        {
            Log.Debug($"Begin webclient request on {uri}");

            var array = client.DownloadData(uri);

            Log.Debug($"Webclient request ended");

            return array;
        }
    }
}
