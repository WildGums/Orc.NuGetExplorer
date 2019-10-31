namespace Orc.NuGetExplorer.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Catel;
    using NuGet.Common;

    public class FileDirectoryService : IFileDirectoryService
    {
        public static readonly string DefaultGlobalPackagesFolderPath = "packages" + Path.DirectorySeparatorChar;

        public string GetGlobalPackagesFolder()
        {
            return Path.Combine(NuGetEnvironment.GetFolderPath(NuGetFolderPath.NuGetHome), DefaultGlobalPackagesFolderPath);
        }

        public string GetApplicationRoamingFolder()
        {
            return Catel.IO.Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserRoaming, Constants.CompanyName, Constants.ProductName);
        }

        public string GetApplicationLocalFolder()
        {
            return Catel.IO.Path.GetApplicationDataDirectory(Catel.IO.ApplicationDataTarget.UserLocal, Constants.CompanyName, Constants.ProductName);
        }

        public void DeleteDirectoryTree(string folderPath, out List<string> failedEntries)
        {
            failedEntries = new List<string>(); //list of directories which causes unavoidable errors during deletion 

            Argument.IsNotNull(() => folderPath);

            if (!Directory.Exists(folderPath))
            {
                //avoid DirectoryNotFoundException and ArgumentException
                return;
            }

            bool fallbackFlag = false;

            try
            {
                //at first try to remove straightforward all directories with files 
                Directory.Delete(folderPath, true);
            }
            catch (UnauthorizedAccessException)
            {
                //the caller does not have the required permission
                fallbackFlag = true;
            }
            catch (PathTooLongException)
            {
                //exceed the system maximum length
                failedEntries.Add(folderPath);
            }
            catch (IOException)
            {
                //msdn Directory.Delete doesn't contains info about causes of this exception
                //but it appeared read-only attributes on file in sub-directories can causes this exceptions
                fallbackFlag = true;
            }

            if (fallbackFlag)
            {
                //we can still delete tree after some preparations
                ForceDeleteFile(folderPath, failedEntries);

                try
                {
                    //retry directory deletion safety
                    Directory.Delete(folderPath, true);
                }
                catch (UnauthorizedAccessException)
                {
                    //the caller does not have the required permission
                    failedEntries.Add(folderPath);
                }
                catch (PathTooLongException)
                {
                    //exceed the system maximum length
                    failedEntries.Add(folderPath);
                }

                catch (IOException)
                {
                    // The directory in use by another process and cause an IOException.
                    // for example by Explorer process handles
                    throw;
                }
            }
        }

        private void ForceDeleteFile(string folderPath, List<string> failedEntries)
        {
            // Using the default SearchOption.TopDirectoryOnly, as SearchOption.AllDirectories would also
            // include reparse points such as mounted drives and symbolic links in the search.
            foreach (var subFolderPath in Directory.EnumerateDirectories(folderPath))
            {
                var directoryInfo = new DirectoryInfo(subFolderPath);

                if (!directoryInfo.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {
                    ForceDeleteFile(subFolderPath, failedEntries);
                }
            }

            foreach (var file in Directory.EnumerateFiles(folderPath))
            {
                var filePath = Path.Combine(folderPath, Path.GetFileName(file));
                try
                {
                    // When files or folders are readonly, the File.Delete method may not be able to delete it.
                    var attributes = File.GetAttributes(filePath);
                    if (attributes.HasFlag(FileAttributes.ReadOnly))
                    {
                        // Remove the readonly flag when set.
                        attributes &= ~FileAttributes.ReadOnly;
                        File.SetAttributes(filePath, attributes);
                    }

                    File.Delete(filePath);
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
        }
    }
}
