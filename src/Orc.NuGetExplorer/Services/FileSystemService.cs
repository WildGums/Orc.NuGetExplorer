// --------------------------------------------------------------------------------------------------------------------
// <copyright file="fileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Catel;
    using Catel.Logging;

    internal class FileSystemService : IFileSystemService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        public bool DeleteDirectory(string path)
        {
            Argument.IsNotNullOrEmpty(() => path);

            Log.Info("Deleting directory {0}", path);

            if (!Directory.Exists(path))
            {
                Log.Warning("Directory {0} does not exists", path);
                return true;
            }

            var files = Directory.GetFiles(path);
            var subDirectories = Directory.GetDirectories(path);

            var failedDirectories = new HashSet<string>();

            var success = DeleteFiles(files, failedDirectories);

            var directories = subDirectories.Except(failedDirectories);
            success = success && DeleteDirectories(path, directories);

            if (success)
            {
                try
                {
                    Directory.Delete(path, false);
                }
                catch (Exception exception)
                {
                    success = false;
                    Log.Error(exception, "Failed to delete directory {0}", path);
                }
            }

            if (success)
            {
                Log.Info("Directory {0} has been successfully deleted", path);
            }

            return success;
        }

        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            Argument.IsNotNullOrEmpty(() => sourceDirectory);
            Argument.IsNotNullOrEmpty(() => destinationDirectory);

            Log.Info("Copying directory {0} to {1}.", sourceDirectory, destinationDirectory);

            var failedDirectories = new HashSet<string>();

            var success = CreateDirectoryStructure(sourceDirectory, destinationDirectory, failedDirectories);

            success = success && CopyFiles(sourceDirectory, destinationDirectory, failedDirectories);

            if (success)
            {
                Log.Info("Copying directory {0} to {1} hes been successully completed.", sourceDirectory, destinationDirectory);
            }
        }

        private static bool CopyFiles(string sourceDirectory, string destinationDirectory, HashSet<string> failedDirectories)
        {
            Argument.IsNotNullOrWhitespace(() => sourceDirectory);
            Argument.IsNotNullOrWhitespace(() => destinationDirectory);
            Argument.IsNotNull(() => failedDirectories);

            bool success = true;
            foreach (var sourceFileName in Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories))
            {
                var destFileName = sourceFileName.Replace(sourceDirectory, destinationDirectory);
                var directoryName = Path.GetDirectoryName(destFileName);
                if (failedDirectories.Contains(directoryName))
                {
                    continue;
                }

                try
                {
                    File.Copy(sourceFileName, destFileName, true);
                }
                catch (Exception exception)
                {
                    success = false;
                    Log.Error(exception, "Failed to copy file {0} to {1}.", sourceFileName, destinationDirectory);
                }
            }
            return success;
        }

        private static bool CreateDirectoryStructure(string sourceDirectory, string destinationDirectory, HashSet<string> failedDirectories)
        {
            Argument.IsNotNullOrWhitespace(() => sourceDirectory);
            Argument.IsNotNullOrWhitespace(() => destinationDirectory);
            Argument.IsNotNull(() => failedDirectories);

            bool success = true;
            var subDirectories = Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories);
            foreach (var dirPath in subDirectories)
            {
                var newDir = dirPath.Replace(sourceDirectory, destinationDirectory);
                try
                {
                    if (!Directory.Exists(newDir))
                    {
                        Directory.CreateDirectory(newDir);
                    }
                }
                catch (Exception exception)
                {
                    success = false;
                    failedDirectories.Add(newDir);
                    Log.Error(exception, "Failed to create directory {0}.", newDir);
                }
            }
            return success;
        }

        private bool DeleteDirectories(string directory, IEnumerable<string> directories)
        {
            Argument.IsNotNullOrWhitespace(() => directory);
            Argument.IsNotNull(() => directories);

            bool success = true;

            foreach (var subDirectory in directories)
            {
                try
                {
                    DeleteDirectory(subDirectory);
                }
                catch (Exception exception)
                {
                    success = false;
                    Log.Error(exception, "Failed to delete directory {0}.", directory);
                }
            }
            return success;
        }

        private static bool DeleteFiles(string[] files, HashSet<string> failedDirectories)
        {
            Argument.IsNotNull(() => files);
            Argument.IsNotNull(() => failedDirectories);

            bool success = true;

            foreach (var file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception exception)
                {
                    success = false;
                    var directoryName = Path.GetDirectoryName(file);
                    failedDirectories.Add(directoryName);
                    Log.Error(exception, "Failed to delete file {0}.", file);
                }
            }
            return success;
        }
        #endregion
    }
}
