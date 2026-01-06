namespace Orc.NuGetExplorer;

using System;
using System.IO;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using Orc.FileSystem;

internal class FileSystemService : IFileSystemService
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(FileSystemService));

    private readonly IFileService _fileService;
    private readonly IDirectoryService _directoryService;

    public FileSystemService(IFileService fileService, IDirectoryService directoryService)
    {
        _fileService = fileService;
        _directoryService = directoryService;
    }

    public void CreateDeleteme(string name, string path)
    {
        Logger.LogDebug($"Creating delete.me file on path '{path}'");

        var fullPath = GetDeletemePath(name, path);
        var directoryPath = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directoryPath))
        {
            Logger.LogDebug("Cannot obtain directory path for creating file.");
            return;
        }

        if (_fileService.Exists(fullPath))
        {
            return;
        }

        _directoryService.Create(directoryPath);

        using (_fileService.Create(fullPath))
        {
            Logger.LogDebug($"Created delete.me file on path {fullPath}");
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
