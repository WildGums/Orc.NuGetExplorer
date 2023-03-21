namespace Orc.NuGetExplorer;

using System.Collections.Generic;
using NuGet.Protocol.Core.Types;

public class InstallerResult
{
    public InstallerResult(string errorMessage)
    {
        ErrorMessage = errorMessage;
        Result = new Dictionary<SourcePackageDependencyInfo, DownloadResourceResult>();
    }

    public InstallerResult(IDictionary<SourcePackageDependencyInfo, DownloadResourceResult> downloadResult)
    {
        Result = downloadResult;
        ErrorMessage = string.Empty;
    }

    public IDictionary<SourcePackageDependencyInfo, DownloadResourceResult> Result { get; private set; }

    public string ErrorMessage { get; private set; }
}