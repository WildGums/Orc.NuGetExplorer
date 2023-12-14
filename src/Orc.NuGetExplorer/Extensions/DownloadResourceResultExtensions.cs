namespace Orc.NuGetExplorer;

using System;
using System.IO;
using NuGet.Protocol.Core.Types;

public static class DownloadResourceResultExtensions
{
    public static string GetResourceRoot(this DownloadResourceResult downloadResourceResult)
    {
        ArgumentNullException.ThrowIfNull(downloadResourceResult);

        if (downloadResourceResult.PackageStream is FileStream fileStream)
        {
            return fileStream.Name;
        }
        else
        {
            return downloadResourceResult.PackageSource ?? string.Empty;
        }
    }

    public static bool IsAvailable(this DownloadResourceResult downloadResourceResult)
    {
        ArgumentNullException.ThrowIfNull(downloadResourceResult);

        return downloadResourceResult.Status is DownloadResourceResultStatus.Available or DownloadResourceResultStatus.AvailableWithoutStream;
    }
}
