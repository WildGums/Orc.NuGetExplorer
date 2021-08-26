namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Catel.Logging;

    internal static class LogHelper
    {
        internal static void LogUnclearedPaths(List<string> unclearedPaths, ILog log)
        {
            if (unclearedPaths?.Any() ?? false)
            {
                log.Info("Some directories cannot be deleted, directory tree was partially cleared:");

                foreach (var failedDelete in unclearedPaths.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
                {
                    log.Info($"Failed to delete path {failedDelete}");
                }
            }
        }
    }
}
