namespace Orc.NuGetExplorer.Web;

using System;
using System.Net;
using Catel.Logging;

public class HttpWebExceptionHandler : IHttpExceptionHandler<WebException>
{
    private static readonly ILog Log = LogManager.GetCurrentClassLogger();

    public FeedVerificationResult HandleException(WebException exception, string source)
    {
        ArgumentNullException.ThrowIfNull(exception);

        try
        {
            var httpWebResponse = (HttpWebResponse?)exception.Response;
            if (httpWebResponse is null)
            {
                return FeedVerificationResult.Invalid;
            }

            // 403 error
            if (httpWebResponse.StatusCode == HttpStatusCode.Forbidden)
            {
                return FeedVerificationResult.AuthorizationRequired;
            }

            // 401 error
            if (httpWebResponse.StatusCode == HttpStatusCode.Unauthorized)
            {
                return FeedVerificationResult.AuthenticationRequired;
            }
        }
        catch (Exception ex)
        {
            Log.Debug(ex, "Failed to verify feed '{0}'", source);
        }

        return FeedVerificationResult.Invalid;
    }
}