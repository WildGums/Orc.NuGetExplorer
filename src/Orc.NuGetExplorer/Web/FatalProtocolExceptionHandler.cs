namespace Orc.NuGetExplorer.Web;

using System;
using System.Net;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Core.Types;

public class FatalProtocolExceptionHandler : IHttpExceptionHandler<FatalProtocolException>
{
    private static readonly ILogger Logger = LogManager.GetLogger(typeof(FatalProtocolExceptionHandler));

    private static readonly IHttpExceptionHandler<WebException> WebExceptionHandler = new HttpWebExceptionHandler();

    public FeedVerificationResult HandleException(FatalProtocolException exception, string source)
    {
        ArgumentNullException.ThrowIfNull(exception);

        try
        {
            var innerException = exception.InnerException;

            if (innerException is null)
            {
                //handle based on protocol error messages
                if (exception.HidesUnauthorizedError())
                {
                    return FeedVerificationResult.AuthenticationRequired;
                }
                if (exception.HidesForbiddenError())
                {
                    return FeedVerificationResult.AuthorizationRequired;
                }
            }
            else
            {
                if (innerException is WebException webException)
                {
                    WebExceptionHandler.HandleException(webException, source);
                }
            }

        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, "Failed to verify feed '{0}'", source);
        }

        return FeedVerificationResult.Invalid;
    }
}
