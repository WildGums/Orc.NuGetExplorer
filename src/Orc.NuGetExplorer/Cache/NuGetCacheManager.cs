namespace Orc.NuGetExplorer.Cache;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using Orc.FileSystem;

public class NuGetCacheManager : INuGetCacheManager, IDisposable
{
    private static readonly Microsoft.Extensions.Logging.ILogger Logger = LogManager.GetLogger(typeof(NuGetCacheManager));

    private readonly SourceCacheContext _sourceContext = new();
    private readonly IDirectoryService _directoryService;
    private readonly IFileService _fileService;
    private bool _disposedValue;

    public NuGetCacheManager(IDirectoryService directoryService, IFileService fileService)
    {
        _directoryService = directoryService;
        _fileService = fileService;
    }

    public bool ClearAll()
    {
        var noErrors = true;
        noErrors &= ClearHttpCache();
        noErrors &= ClearNuGetFolder(DefaultNuGetFolders.GetGlobalPackagesFolder(), "Global-packages");
        noErrors &= ClearNuGetFolder(NuGetEnvironment.GetFolderPath(NuGetFolderPath.Temp), "Temp");

        Logger.LogInformation("Cache clearing operation finished");

        return noErrors;
    }

    public bool ClearHttpCache()
    {
        return ClearNuGetFolder(SettingsUtility.GetHttpCacheFolder(), "Http-cache");
    }

    public HttpSourceCacheContext GetHttpCacheContext(int retryCount, bool directDownload = false)
    {
        // create http cache context from source cache instance
        var baseCache = _sourceContext;

        if (directDownload)
        {
            baseCache = _sourceContext.Clone();
            baseCache.DirectDownload = directDownload;
        }

        return HttpSourceCacheContext.Create(baseCache, retryCount);
    }

    public HttpSourceCacheContext GetHttpCacheContext()
    {
        return GetHttpCacheContext(0);
    }

    public SourceCacheContext GetCacheContext()
    {
        return _sourceContext;
    }

    private bool ClearNuGetFolder(string folderPath, string folderDescription)
    {
        var success = true;

        if (!string.IsNullOrEmpty(folderPath))
        {
            Logger.LogInformation("Clear {FolderDescription} folder on path {FolderPath}", folderDescription, folderPath);

            success &= ClearCacheDirectory(folderPath);
        }

        return success;
    }

    private bool ClearCacheDirectory(string folderPath)
    {
        var failedDeletes = new List<string>();

        try
        {
            _directoryService.ForceDeleteDirectory(_fileService, folderPath, out failedDeletes);
        }
        catch (UnauthorizedAccessException)
        {
            Logger.LogWarning("Cache clear ended unsuccessfully, directory is in use by another process");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Cache clear ended unsuccessfully");
        }
        finally
        {
            // log all errors

            LogHelper.LogUnclearedPaths(failedDeletes, Logger);
        }

        return !failedDeletes.Any();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            _sourceContext?.Dispose();
        }

        _disposedValue = true;
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
