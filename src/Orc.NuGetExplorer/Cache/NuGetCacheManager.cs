namespace Orc.NuGetExplorer.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using Orc.FileSystem;

    public class NuGetCacheManager : INuGetCacheManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        //private readonly IFileDirectoryService _fileDirectoryService;
        private readonly SourceCacheContext _sourceContext = new SourceCacheContext();
        private readonly IDirectoryService _directoryService;
        private readonly IFileService _fileService;

        public NuGetCacheManager(IDirectoryService directoryService, IFileService fileService)
        {
            Argument.IsNotNull(() => directoryService);
            Argument.IsNotNull(() => fileService);
            _directoryService = directoryService;
            _fileService = fileService;
        }

        public bool ClearAll()
        {
            bool noErrors = true;
            noErrors &= ClearHttpCache();
            noErrors &= ClearNuGetFolder(DefaultNuGetFolders.GetGlobalPackagesFolder(), "Global-packages");
            noErrors &= ClearNuGetFolder(NuGetEnvironment.GetFolderPath(NuGetFolderPath.Temp), "Temp");

            Log.Info("Cache clearing operation finished");

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
                Log.Info($"Clear {folderDescription} folder on path {folderPath}");

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
            catch (Exception)
            {
                Log.Error("Cache clear ended unsuccessfully, directory is in use by another process");
            }
            finally
            {
                // log all errors

                LogHelper.LogUnclearedPaths(failedDeletes, Log);
            }

            return !failedDeletes.Any();
        }
    }
}
