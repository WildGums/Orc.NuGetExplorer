// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackupFileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    internal interface IBackupFileSystemService
    {
        #region Methods
        void BackupFolder(string fullPath);
        void Restore(string fullPath);
        #endregion
    }
}