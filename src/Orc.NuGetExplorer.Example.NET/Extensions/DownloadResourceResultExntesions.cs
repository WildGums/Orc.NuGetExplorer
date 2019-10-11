namespace Orc.NuGetExplorer
{
    using Catel;
    using NuGet.Protocol.Core.Types;
    using System;
    using System.IO;

    public static class DownloadResourceResultExntesions
    {
        public static string GetResourceRoot(this DownloadResourceResult downloadResourceResult)
        {
            Argument.IsNotNull(() => downloadResourceResult);

            var fileStream = downloadResourceResult.PackageStream as FileStream;

            if (fileStream != null)
            {
                return fileStream.Name;
            }
            else
            {
                return downloadResourceResult.PackageSource ?? String.Empty;
            }
        }
    }
}
