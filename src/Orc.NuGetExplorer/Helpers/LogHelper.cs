namespace Orc.NuGetExplorer;

using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Logging;
using Microsoft.Extensions.Logging;

public class LogHelper
{
    public static void LogUnclearedPaths(List<string>? unclearedPaths, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (unclearedPaths?.Any() ?? false)
        {
            logger.LogInformation("Some directories cannot be deleted, directory tree was partially cleared:");

            foreach (var failedDelete in unclearedPaths.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                logger.LogInformation($"Failed to delete path {failedDelete}");
            }
        }
    }
}
