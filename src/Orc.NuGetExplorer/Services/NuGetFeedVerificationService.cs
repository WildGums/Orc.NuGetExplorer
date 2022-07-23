
namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Protocol.Core.Types;
    using NuGetExplorer.Web;

    internal class NuGetFeedVerificationService : INuGetFeedVerificationService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        private static readonly IHttpExceptionHandler<WebException> WebExceptionHandler = new HttpWebExceptionHandler();
        private static readonly IHttpExceptionHandler<FatalProtocolException> FatalProtocolExceptionHandler = new FatalProtocolExceptionHandler();

        private readonly ILogger _nugetLogger;
        private readonly ISourceRepositoryProvider _repositoryProvider;

        public NuGetFeedVerificationService(ISourceRepositoryProvider repositoryProvider, ILogger logger)
        {
            Argument.IsNotNull(() => repositoryProvider);
            Argument.IsNotNull(() => logger);

            _repositoryProvider = repositoryProvider;
            _nugetLogger = logger;
        }

        public async Task<FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = false, CancellationToken cancellationToken = default)
        {
            Argument.IsNotNull(() => source);

            var result = FeedVerificationResult.Valid;

            var errorMessage = new StringBuilder($"Failed to verify feed '{source}'");

            Log.Debug("Verifying feed '{0}'", source);

            try
            {
                var packageSource = new PackageSource(source);

                var repository = _repositoryProvider.CreateRepository(packageSource);

                try
                {
                    var searchResource = await repository.GetResourceAsync<PackageSearchResource>();

                    var metadata = await searchResource.SearchAsync(string.Empty, new SearchFilter(false), 0, 1, _nugetLogger, cancellationToken);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (FatalProtocolException ex)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    //cancel operation
                    throw new OperationCanceledException("Verification was canceled", ex, cancellationToken);
                }
                result = FatalProtocolExceptionHandler.HandleException(ex, source);
            }
            catch (WebException ex)
            {
                result = WebExceptionHandler.HandleException(ex, source);
            }
            catch (UriFormatException ex)
            {
                errorMessage.Append(", a UriFormatException occurred");
                Log.Debug(ex, errorMessage.ToString());

                result = FeedVerificationResult.Invalid;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                Log.Debug(ex, errorMessage.ToString());

                result = FeedVerificationResult.Invalid;
            }

            Log.Debug("Verified feed '{0}', result is '{1}'", source, result);

            return result;
        }    
    }
}
