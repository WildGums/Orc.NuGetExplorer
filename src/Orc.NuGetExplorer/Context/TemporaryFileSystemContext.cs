// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFileSystemContext.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;

    public class TemporaryFileSystemContext : ITemporaryFileSystemContext
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IFIleSystemService _fIleSystemService;
        private readonly string _rootDirectory;
        #endregion

        #region Constructors
        public TemporaryFileSystemContext(IFIleSystemService fIleSystemService)
        {
            Argument.IsNotNull(() => fIleSystemService);

            _fIleSystemService = fIleSystemService;

            var assembly = AssemblyHelper.GetEntryAssembly();

            _rootDirectory = Path.Combine(Path.GetTempPath(), assembly.Company(), assembly.Title(),
                "backup", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            Directory.CreateDirectory(_rootDirectory);
        }
        #endregion

        #region Properties
        public string RootDirectory
        {
            get { return _rootDirectory; }
        }
        #endregion

        #region Methods
        public void Dispose()
        {
            Log.Info("Deleting temporary files from '{0}'", _rootDirectory);

            if (!_fIleSystemService.DeleteDirectory(_rootDirectory))
            {
                Log.Error("Failed to delete temporary files");
            }
            else
            {
                Log.Info("Temporary files has been successfully deleted from '{0}'", _rootDirectory);
            }
        }

        public string GetDirectory(string relativeDirectoryName)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeDirectoryName);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
            }

            return fullPath;
        }

        public string GetFile(string relativeFilePath)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeFilePath);

            var directory = Path.GetDirectoryName(fullPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return fullPath;
        }
        #endregion
    }
}