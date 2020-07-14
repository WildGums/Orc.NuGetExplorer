namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Threading.Tasks;
    using System.Timers;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Packaging.Core;
    using NuGet.Protocol.Core.Types;

    public class DownloadingProgressTrackerService : IDownloadingProgressTrackerService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly ILogger _nugetLogger;

        public DownloadingProgressTrackerService(ILogger nugetLogger)
        {
            Argument.IsNotNull(() => nugetLogger);
            _nugetLogger = nugetLogger;
        }

        public async Task<IDisposableToken<IProgress<float>>> TrackDownloadOperationAsync(IPackageInstallationService packageInstallationService, SourcePackageDependencyInfo packageDependencyInfo)
        {
            Argument.IsNotNull(() => packageInstallationService);
            Argument.IsNotNull(() => packageDependencyInfo);

            try
            {
                FileSystemWatcher watcher = new FileSystemWatcher();
                var downloadPath = packageInstallationService.InstallerPathResolver.GetPackageFilePath(packageDependencyInfo.Id, packageDependencyInfo.Version);
                var downloadDirectoryPath = Path.GetDirectoryName(downloadPath);

                // the download method creates directory itself, but we need to create it eager to start watching
                if (!Directory.Exists(downloadDirectoryPath))
                {
                    Directory.CreateDirectory(downloadDirectoryPath);
                }

                watcher.Path = Path.GetDirectoryName(downloadPath);
                // determine package size
                var packageByteSize = await packageInstallationService.MeasurePackageSizeFromRepositoryAsync(packageDependencyInfo, packageDependencyInfo.Source);
                var trackToken = new DownloadProgressTrackToken(this, packageDependencyInfo, packageDependencyInfo.Source, watcher, OnProgressReportedCallback, packageByteSize ?? 0);

                return trackToken;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        private void OnProgressReportedCallback(object sender, float progress)
        {
            if (double.IsNaN(progress))
            {
                _nugetLogger.LogInformation($"[Please wait]Download complete");
            }
            _nugetLogger.LogInformation($"[Please wait]Download in progress.. {progress / (1024 * 1024):0.##} Mb");
        }
    }

    internal class DownloadProgressTrackToken : DisposableToken<IProgress<float>>
    {
        private readonly long _downloadSize;
        private readonly FileSystemWatcher _fileSystemWatcher;
        private readonly Timer _trackerTimer;

        private string _nupkgFilePath;

        public DownloadProgressTrackToken(IDownloadingProgressTrackerService downloadingProgressTrackerService, PackageIdentity packageIdentity, SourceRepository source, FileSystemWatcher fileSystemWatcher,
            EventHandler<float> progressCallback, long downloadSize) :
            this(InitializeInstance(progressCallback), (token) => token.Instance.Report(0f), (token) => token.Instance.Report(1f))
        {
            Argument.IsNotNull(() => downloadingProgressTrackerService);
            Argument.IsNotNull(() => packageIdentity);
            Argument.IsNotNull(() => source);
            Argument.IsNotNull(() => fileSystemWatcher);

            _downloadSize = downloadSize;
            DownloadingProgressTrackerInstance = downloadingProgressTrackerService;
            PackageIdentity = packageIdentity;
            SourceRepository = source;
            _fileSystemWatcher = fileSystemWatcher;
            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.Created += OnFileSystemCreated;
            _fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.Size;

            _trackerTimer = new Timer(2500);
            _trackerTimer.Elapsed += OnTrackerTimerElapsed;
        }

        public DownloadProgressTrackToken(IProgress<float> instance, Action<IDisposableToken<IProgress<float>>> initialize, Action<IDisposableToken<IProgress<float>>> dispose, object tag = null)
            : base(instance, initialize, dispose, tag)
        {
        }

        public IDownloadingProgressTrackerService DownloadingProgressTrackerInstance { get; }

        public SourceRepository SourceRepository { get; }

        public PackageIdentity PackageIdentity { get; }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();
            if (_fileSystemWatcher != null)
            {
                _fileSystemWatcher.Dispose();
            }

            if (_trackerTimer.Enabled)
            {
                _trackerTimer.Stop();
            }
        }

        private static Progress<float> InitializeInstance(EventHandler<float> onProgressChangedCallback)
        {
            var progress = new Progress<float>();
            progress.ProgressChanged += onProgressChangedCallback;

            return progress;
        }

        private void OnFileSystemCreated(object sender, FileSystemEventArgs e)
        {
            if (!_trackerTimer.Enabled)
            {
                _nupkgFilePath = e.FullPath;
                _trackerTimer.Start();
            }
        }

        private void OnTrackerTimerElapsed(object sender, ElapsedEventArgs e)
        {
            if (File.Exists(_nupkgFilePath))
            {
                var fileInfo = new FileInfo(_nupkgFilePath);
                var mbSize = fileInfo.Length;
                Instance.Report(UpdateProgress(mbSize));
            }
        }

        private float UpdateProgress(long currentSize)
        {
            if (_downloadSize == 0 || double.IsInfinity(_downloadSize) || double.IsNaN(_downloadSize))
            {
                return currentSize;
            }
            return (float)currentSize / _downloadSize;
        }
    }
}
