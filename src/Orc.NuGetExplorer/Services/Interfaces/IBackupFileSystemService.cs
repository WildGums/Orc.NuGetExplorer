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
