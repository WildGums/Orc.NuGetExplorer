
namespace Orc.NuGetExplorer
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
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
        private readonly ICredentialProviderLoaderService _credentialProviderLoaderService;
        private readonly ISourceRepositoryProvider _repositoryProvider;

        public NuGetFeedVerificationService(ICredentialProviderLoaderService credentialProviderLoaderService, ISourceRepositoryProvider repositoryProvider, ILogger logger)
        {
            _credentialProviderLoaderService = credentialProviderLoaderService;
            _repositoryProvider = repositoryProvider;
            _nugetLogger = logger;
        }

        public async Task<FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = false, CancellationToken cancellationToken = default)
        {
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

        [ObsoleteEx]
        public FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true)
        {
            var timeOut = 3000;

            var result = FeedVerificationResult.Valid;

            var errorMessage = new StringBuilder($"Failed to verify feed '{source}'");

            Log.Debug("Verifying feed '{0}'", source);

            try
            {
                var packageSource = new PackageSource(source);

                var repository = _repositoryProvider.CreateRepository(packageSource);

                var searchResource = repository.GetResource<PackageSearchResource>();

                using (var cts = new CancellationTokenSource())
                {
                    var cancellationToken = cts.Token;

                    //try to perform search
                    var searchTask = searchResource.SearchAsync(String.Empty, new SearchFilter(false), 0, 1, _nugetLogger, cancellationToken);

                    var searchCompletion = Task.WhenAny(searchTask, Task.Delay(timeOut, cancellationToken)).Result;

                    if (searchCompletion != searchTask)
                    {
                        throw new TimeoutException("Search operation has timed out");
                    }

                    if (searchTask.IsFaulted && searchTask.Exception is not null)
                    {
                        throw searchTask.Exception;
                    }
                    if (searchTask.IsCanceled)
                    {
                        return FeedVerificationResult.Unknown;
                    }
                }
            }
            catch (FatalProtocolException ex)
            {
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
            catch (Exception ex)
            {
                Log.Debug(ex, errorMessage.ToString());

                result = FeedVerificationResult.Invalid;
            }

            Log.Debug("Verified feed '{0}', result is '{1}'", source, result);

            return result;
        }
    }
}
