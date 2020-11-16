namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel;
    using Orc.FileSystem;

    public static class IDirectoryServiceExtensions
    {
        public static void ForceDeleteDirectory(this IDirectoryService directoryService, IFileService fileService, string folderPath, out List<string> failedEntries)
        {
            Argument.IsNotNull(() => directoryService);
            Argument.IsNotNull(() => folderPath);

            failedEntries = new List<string>(); //list of directories which cause unavoidable errors during deletion 
            var fallbackFlag = false;

            try
            {
                directoryService.Delete(folderPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                // Caller doesn't have enough permissions
                fallbackFlag = true;
            }
            catch (PathTooLongException)
            {
                // Exceed the system maximum length
                failedEntries.Add(folderPath);
            }
            catch (IOException)
            {
                // Directory.Delete has multiple causes of exception
                // We try to handle read-only attributes on file in sub-directories which can cause this exceptions
                fallbackFlag = true;
            }

            // Do some preparations and try again
            if (fallbackFlag)
            {
                var directoryStack = new Stack<string>();

                directoryStack.Push(folderPath);

                while (directoryStack.Count > 0)
                {
                    var path = directoryStack.Pop();

                    // Using the default SearchOption.TopDirectoryOnly, as SearchOption.AllDirectories would also
                    // include reparse points such as mounted drives and symbolic links in the search.
                    foreach (var subFolderPath in directoryService.GetDirectories(path, searchOption: SearchOption.TopDirectoryOnly))
                    {
                        directoryStack.Push(subFolderPath);
                    }

                    var directoryInfo = new DirectoryInfo(path);

                    if (!directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                    {
                        foreach (var filePath in directoryService.GetFiles(path))
                        {
                            fileService.ForceDeleteFiles(filePath, failedEntries);
                        }
                    }
                }

                try
                {
                    Directory.Delete(folderPath, true);
                }
                catch (UnauthorizedAccessException)
                {
                    // The caller does not have the required permission
                    failedEntries.Add(folderPath);
                }
                catch (PathTooLongException)
                {
                    // Exceed the system maximum length
                    failedEntries.Add(folderPath);
                }

                catch (IOException)
                {
                    throw;
                }
            }
        }
    }
}
