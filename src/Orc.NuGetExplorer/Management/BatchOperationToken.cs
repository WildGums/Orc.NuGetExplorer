namespace Orc.NuGetExplorer.Management;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Logging;

internal partial class NuGetProjectPackageManager
{
    private sealed class BatchOperationToken : IDisposable
    {
        private readonly List<NuGetProjectEventArgs> _supressedInvokationEventArgs = new();

        public void Add(NuGetProjectEventArgs eventArgs)
        {
            _supressedInvokationEventArgs.Add(eventArgs);
        }

        public bool IsDisposed { get; private set; }

        public IEnumerable<T> GetInvokationList<T>()
            where T : NuGetProjectEventArgs
        {
            if (_supressedInvokationEventArgs.All(args => args is T))
            {
                return _supressedInvokationEventArgs.Cast<T>();
            }

            Log.Warning("Mixed batched event args");
            return Enumerable.Empty<T>();
        }

        public void Dispose()
        {
            var last = _supressedInvokationEventArgs.LastOrDefault();

            if (last is not null)
            {
                switch (last)
                {
                    case BatchedInstallNuGetProjectEventArgs b:
                        b.IsBatchEnd = true;
                        break;

                    case BatchedUninstallNuGetProjectEventArgs b:
                        b.IsBatchEnd = true;
                        break;
                }
            }

            IsDisposed = true;
        }
    }
}