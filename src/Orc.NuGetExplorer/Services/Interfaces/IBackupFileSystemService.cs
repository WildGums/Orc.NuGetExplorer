namespace Orc.NuGetExplorer
{
    public interface IBackupFileSystemService
    {
        void BackupFolder(string fullPath);
        void BackupFile(string filePath);
        void Restore(string fullPath);
    }
}
