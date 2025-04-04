﻿namespace Orc.NuGetExplorer.Web;

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

    public static async Task<byte[]> GetByUrlAsync(Uri uri, WebClient client)
    {
        Log.Debug($"Webclient request on {uri}");

        var array = await client.DownloadDataTaskAsync(uri);

        return array;
    }

    public static byte[] GetByUrl(Uri uri, WebClient client)
    {
        Log.Debug($"Webclient request on {uri}");

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
