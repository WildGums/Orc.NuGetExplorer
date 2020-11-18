namespace Orc.NuGetExplorer
{
    using System;
    using System.IO;
    using Catel.Logging;

    internal class FileSystemService : IFileSystemService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Methods

        public void CreateDeleteme(string name, string path)
        {
            var fullPath = GetDeletemePath(name, path);
            var directoryPath = Path.GetDirectoryName(fullPath);

            try
            {
                if (File.Exists(fullPath))
                {
                    return;
                }

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                using (File.Create(fullPath))
                {
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                Log.Info($"Cannot create requested deleteme file on path {fullPath}");
            }
        }

        public void RemoveDeleteme(string name, string path)
        {
            var fullPath = GetDeletemePath(name, path);

            try
            {
                if (!File.Exists(fullPath))
                {
                    return;
                }

                File.Delete(fullPath);
            }
            catch(Exception ex)
            {
                Log.Error(ex);
                Log.Info($"Cannot remove deleteme file on path {fullPath}");
            }
        }

        private static string GetDeletemePath(string name, string path)
        {
            return Path.Combine(path, $"{name}.deleteme");
        }
        #endregion
    }
}
