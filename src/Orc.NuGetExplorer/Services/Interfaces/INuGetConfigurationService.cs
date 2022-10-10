namespace Orc.NuGetExplorer
{
    using System.Collections.Generic;

    public interface INuGetConfigurationService
    {
        #region Methods
        string GetDestinationFolder();
        void SetDestinationFolder(string value);
        IEnumerable<IPackageSource> LoadPackageSources(bool onlyEnabled = false);

        bool SavePackageSource(string name, string source, bool isEnabled = true, bool isOfficial = true, bool verifyFeed = true);

        void DisablePackageSource(string name, string source);
        void SavePackageSources(IEnumerable<IPackageSource> packageSources);

        void SaveProjects(IEnumerable<IExtensibleProject> extensibleProjects);

        void SetPackageQuerySize(int size);
        int GetPackageQuerySize();

        void SetIsPrereleaseAllowed(IRepository repository, bool value);
        bool GetIsPrereleaseAllowed(IRepository repository);

        void RemovePackageSource(IPackageSource source);

        bool IsProjectConfigured(IExtensibleProject project);
        #endregion
    }
}
