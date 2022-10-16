namespace Orc.NuGetExplorer.Watchers.Base
{
    using System;
    using System.Linq;
    using NuGet.Configuration;

    public abstract class PackageSourcesWatcherBase
    {
        protected PackageSourcesWatcherBase(IPackageSourceProvider packageSourceProvider)
        {
            ArgumentNullException.ThrowIfNull(packageSourceProvider);

            packageSourceProvider.PackageSourcesChanged += OnPackageSourcesChanged;
        }

        private void OnPackageSourcesChanged(object? sender, EventArgs e)
        {
            if (sender is IPackageSourceProvider provider)
            {
                var packageSource = provider.LoadPackageSources().FirstOrDefault(source => !string.IsNullOrWhiteSpace(source.Source));

                OnPackageSourcesChanged(packageSource?.Source);
            }
        }

        protected virtual void OnPackageSourcesChanged(string? packageSource)
        {

        }
    }
}
