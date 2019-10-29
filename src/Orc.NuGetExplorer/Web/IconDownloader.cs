namespace Orc.NuGetExplorer.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Catel.Logging;

    public class IconDownloader
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private int _count = 0;

        public IconDownloader()
        {
            //this can be danger and considered harmful (we trust to all cerfs)
            //more info https://stackoverflow.com/questions/703272/could-not-establish-trust-relationship-for-ssl-tls-secure-channel-soap/6613434#6613434
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public async Task<byte[]> GetByUrlAsync(Uri uri, WebClient client)
        {
            _count += 1;

            Log.Debug($"Begin webclient request {_count} on {uri}");

            //while (client.IsBusy)
            //{
            //    Log.Debug("Waiting for ending current download");
            //    await Task.Delay(200);
            //}
            var array = await client.DownloadDataTaskAsync(uri);

            Log.Debug($"Webclient request {_count} ended");

            return array;
        }
    }
}
