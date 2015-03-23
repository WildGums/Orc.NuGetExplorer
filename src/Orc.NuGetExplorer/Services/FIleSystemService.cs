// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FIleSystemService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel.Logging;

    public class FIleSystemService : IFIleSystemService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods
        public void DeleteDirectory(string directory)
        {
            var files = Directory.GetFiles(directory);
            var subDirectories = Directory.GetDirectories(directory);

            foreach (var file in files)
            {
                try
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }
                catch (Exception exception)
                {
                    Log.Error(exception);
                }
            }

            foreach (var subDirectory in subDirectories)
            {
                DeleteDirectory(subDirectory);
            }

            try
            {
                Directory.Delete(directory, false);
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }
            
        }

        public void CopyDirectory(string sourceDirectory, string destinationDirectory)
        {
            foreach (var dirPath in Directory.GetDirectories(sourceDirectory, "*", SearchOption.AllDirectories))
            {
                var newDir = dirPath.Replace(sourceDirectory, destinationDirectory);
                try
                {                    
                    Directory.CreateDirectory(newDir);
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Failed to create directory {0}", newDir);
                }                
            }

            foreach (var sourceFileName in Directory.GetFiles(sourceDirectory, "*.*", SearchOption.AllDirectories))
            {
                var destFileName = sourceFileName.Replace(sourceDirectory, destinationDirectory);
                try
                {                    
                    File.Copy(sourceFileName, destFileName, true);
                }
                catch (Exception exception)
                {
                    Log.Error(exception, "Failed to copy file {0} to {1}", sourceFileName, destinationDirectory);
                }                
            }
        }
        #endregion
    }
}