namespace Orc.NuGetExplorer
{
    using System.IO;
    using Catel;
    using Catel.Logging;
    using Orc.FileSystem;

    internal class FileSystemService : IFileSystemService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IFileService _fileService;
        private readonly IDirectoryService _directoryService;

        public FileSystemService(IFileService fileService, IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => fileService);
            Argument.IsNotNull(() => directoryService);

            _fileService = fileService;
            _directoryService = directoryService;
        }

        public void CreateDeleteme(string name, string path)
        {
            Log.Debug($"Createing delete.me file on path '{path}'");

            var fullPath = GetDeletemePath(name, path);
            var directoryPath = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(directoryPath))
            {
                Log.Debug("Cannot obtain directory path for creating file.");
                return;
            }

            if (_fileService.Exists(fullPath))
            {
                return;
            }

            _directoryService.Create(directoryPath);

            using (_fileService.Create(fullPath))
            {
                Log.Debug($"Created delete.me file on path {fullPath}");
            }
        }

        public void RemoveDeleteme(string name, string path)
        {
            var fullPath = GetDeletemePath(name, path);
            {
                _fileService.Delete(fullPath);
            }
        }

        private static string GetDeletemePath(string name, string path)
        {
            return Path.Combine(path, $"{name}.deleteme");
        }
    }
}
