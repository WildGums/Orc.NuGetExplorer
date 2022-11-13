namespace Orc.NuGetExplorer
{
    public interface IFileSystemService
    {
        void CreateDeleteme(string name, string path);
        void RemoveDeleteme(string name, string path);
    }
}
