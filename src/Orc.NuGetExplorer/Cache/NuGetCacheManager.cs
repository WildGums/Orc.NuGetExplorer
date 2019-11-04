namespace Orc.NuGetExplorer.Cache
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using Orc.NuGetExplorer.Loggers;
    using Orc.NuGetExplorer.Services;

    public class NuGetCacheManager : INuGetCacheManager
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IFileDirectoryService _fileDirectoryService;
        private readonly SourceCacheContext _sourceContext = new SourceCacheContext();

        public NuGetCacheManager(IFileDirectoryService fileDirectoryService)
        {
            Argument.IsNotNull(() => fileDirectoryService);

            _fileDirectoryService = fileDirectoryService;
        }

        public bool ClearAll()
        {
            bool noErrors = true;
            noErrors &= ClearNuGetFolder(SettingsUtility.GetHttpCacheFolder(), "Http-cache");
            noErrors &= ClearNuGetFolder(_fileDirectoryService.GetGlobalPackagesFolder(), "Global-packages");
            noErrors &= ClearNuGetFolder(NuGetEnvironment.GetFolderPath(NuGetFolderPath.Temp), "Temp");

            Log.Info("Cache clearing operation finished");

            return noErrors;
        }

        public HttpSourceCacheContext GetHttpCacheContext(int retryCount, bool directDownload = false)
        {
            //create http cache context from source cache instance
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
            List<string> failedDeletes = new List<string>();

            try
            {
                _fileDirectoryService.DeleteDirectoryTree(folderPath, out failedDeletes);
            }
            catch (IOException)
            {
                Log.Error("Cache clear ended unsuccessfully, directory is in use by another process");
            }
            finally
            {
                //log all errors

                LogHelper.LogUnclearedPaths(failedDeletes, Log);
            }

            return !failedDeletes.Any();
        }
    }
}
