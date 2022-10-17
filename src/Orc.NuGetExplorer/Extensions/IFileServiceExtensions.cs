namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Orc.FileSystem;

    public static class IFileServiceExtensions
    {
        public static void ForceDeleteFiles(this IFileService fileService, string filePath, List<string> failedEntries)
        {
            try
            {
                // When files or folders are readonly, the File.Delete method may not be able to delete it.
                SetAttributes(fileService, filePath, FileAttributes.ReadOnly);
                fileService.Delete(filePath);
            }
            catch (PathTooLongException)
            {
                failedEntries.Add(filePath);
            }
            catch (UnauthorizedAccessException)
            {
                failedEntries.Add(filePath);
            }
            catch (IOException)
            {
                // The file is being used by another process.
                failedEntries.Add(filePath);
            }
        }

        public static void SetAttributes(this IFileService fileService, string filePath, FileAttributes attribute)
        {
            var attributes = File.GetAttributes(filePath);
            if (attributes.HasFlag(attribute))
            {
                // Remove flag when set.
                attributes &= ~attribute;
                File.SetAttributes(filePath, attributes);
            }
        }
    }
}
