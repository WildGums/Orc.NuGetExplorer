namespace Orc.NuGetExplorer.Web;

using System;
using System.Net;
using System.Threading.Tasks;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class IconDownloader
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(IconDownloader));

    public IconDownloader()
    {
        SetProtocolSecurity();
    }

    public static async Task<byte[]> GetByUrlAsync(Uri uri, WebClient client)
    {
        Logger.LogDebug("Webclient request on {Uri}", uri);

        var array = await client.DownloadDataTaskAsync(uri);

        return array;
    }

    public static byte[] GetByUrl(Uri uri, WebClient client)
    {
        Logger.LogDebug("Webclient request on {Uri}", uri);

        var array = client.DownloadData(uri);

        return array;
    }

    private static void SetProtocolSecurity()
    {
#if NET8
        // Note: ignore if not .net 9 or higher
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
#endif
    }
}
