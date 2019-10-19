namespace Orc.NuGetExplorer.Web
{
    using Catel.Logging;
    using NuGet.Protocol.Core.Types;
    using System;
    using System.Net;

    public class FatalProtocolExceptionHandler : IHttpExceptionHandler<FatalProtocolException>
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private static readonly IHttpExceptionHandler<WebException> WebExceptionHandler = new HttpWebExceptionHandler();

        public FeedVerificationResult HandleException(FatalProtocolException exception, string source)
        {
            try
            {
                var innerException = exception.InnerException;

                if (innerException == null)
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
                    if (innerException is WebException)
                    {
                        WebExceptionHandler.HandleException(innerException as WebException, source);
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to verify feed '{0}'", source);
            }

            return FeedVerificationResult.Invalid;
        }
    }
}
