namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.Linq;
using NuGet.Packaging.Core;

internal partial class NuGetProjectPackageManager
{
    private sealed class BatchUpdateToken : IDisposable
    {
        private readonly List<NuGetProjectEventArgs> _supressedInvokationEventArgs = new();

        private readonly PackageIdentity _identity;

        public BatchUpdateToken(PackageIdentity identity)
        {
            ArgumentNullException.ThrowIfNull(identity);

            _identity = identity;
        }

        public bool IsDisposed { get; private set; }

        public void Add(NuGetProjectEventArgs eventArgs)
        {
            _supressedInvokationEventArgs.Add(eventArgs);
        }

        public IEnumerable<UpdateNuGetProjectEventArgs> GetUpdateEventArgs()
        {
            return _supressedInvokationEventArgs
                .GroupBy(e => new
                {
                    e.Package.Id,
                    e.Project
                })
                .Select(group => new UpdateNuGetProjectEventArgs(group.Key.Project, _identity, group))
                .ToList();
        }

        public void Dispose()
        {
            IsDisposed = true;
        }
    }
}