// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBackupFileSystemService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
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