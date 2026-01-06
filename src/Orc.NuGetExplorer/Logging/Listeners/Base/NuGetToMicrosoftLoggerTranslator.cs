namespace Orc.NuGetExplorer;

using Catel.Logging;
using Microsoft.Extensions.Logging;

public class NuGetToMicrosoftLoggerTranslator : PackageManagerLogListenerBase
{
    private readonly ILogger<NuGetToMicrosoftLoggerTranslator> _logger;

    public NuGetToMicrosoftLoggerTranslator(ILogger<NuGetToMicrosoftLoggerTranslator> logger,
        INuGetLogListeningService nuGetLogListeningService)
        : base(nuGetLogListeningService)
    {
        _logger = logger;
    }

    protected override void OnInfo(object? sender, NuGetLogRecordEventArgs e)
    {
        _logger.LogInformation(e.Message);
    }

    protected override void OnDebug(object? sender, NuGetLogRecordEventArgs e)
    {
        _logger.LogDebug(e.Message);
    }

    protected override void OnWarning(object? sender, NuGetLogRecordEventArgs e)
    {
        _logger.LogWarning(e.Message);
    }

    protected override void OnError(object? sender, NuGetLogRecordEventArgs e)
    {
        _logger.LogError(e.Message);
    }
}
