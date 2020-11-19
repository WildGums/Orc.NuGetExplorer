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
            var fullPath = GetDeletemePath(name, path);
            var directoryPath = Path.GetDirectoryName(fullPath);

            if (_fileService.Exists(fullPath))
            {
                return;
            }

            _directoryService.Create(directoryPath);

            using (_fileService.Create(fullPath))
            {
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
