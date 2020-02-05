// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackupFileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IBackupFileSystemService
    {
        #region Methods
        void BackupFolder(string fullPath);
        void BackupFile(string filePath);
        void Restore(string fullPath);
        #endregion
    }
}
