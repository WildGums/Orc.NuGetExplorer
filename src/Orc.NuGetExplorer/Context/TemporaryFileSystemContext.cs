// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporaryFileSystemContext.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel;
    using Catel.Logging;
    using Catel.Reflection;
    using Orc.FileSystem;

    internal class TemporaryFileSystemContext : ITemporaryFileSystemContext
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private readonly IDirectoryService _directoryService;
        private readonly string _rootDirectory;
        #endregion

        #region Constructors
        public TemporaryFileSystemContext(IDirectoryService directoryService)
        {
            Argument.IsNotNull(() => directoryService);

            _directoryService = directoryService;

            var assembly = AssemblyHelper.GetEntryAssembly();

            _rootDirectory = Path.Combine(Path.GetTempPath(), assembly.Company(), assembly.Title(),
                "backup", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            _directoryService.Create(_rootDirectory);
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                Log.Info("Deleting temporary files from '{0}'", _rootDirectory);

                _directoryService.Delete(_rootDirectory, true);

                Log.Info("Temporary files has been successfully deleted from '{0}'", _rootDirectory);
            }
            catch (Exception)
            {
                Log.Warning("Unable to cleanup temporary files");
            }
        }

        public string GetDirectory(string relativeDirectoryName)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeDirectoryName);

            if (!_directoryService.Exists(fullPath))
            {
                _directoryService.Create(fullPath);
            }

            return fullPath;
        }

        public string GetFile(string relativeFilePath)
        {
            var fullPath = Path.Combine(_rootDirectory, relativeFilePath);
            var directory = Path.GetDirectoryName(fullPath);

            if (!_directoryService.Exists(directory))
            {
                _directoryService.Create(directory);
            }

            return fullPath;
        }
        #endregion
    }
}
