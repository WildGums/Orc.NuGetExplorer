
namespace Orc.NuGetExplorer;

using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Catel;
using Catel.Logging;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGetExplorer.Web;

internal class NuGetFeedVerificationService : INuGetFeedVerificationService
{
    private static readonly Microsoft.Extensions.Logging.ILogger Logger = LogManager.GetLogger(typeof(NuGetFeedVerificationService));

    private static readonly IHttpExceptionHandler<WebException> WebExceptionHandler = new HttpWebExceptionHandler();
    private static readonly IHttpExceptionHandler<FatalProtocolException> FatalProtocolExceptionHandler = new FatalProtocolExceptionHandler();

    private readonly NuGet.Common.ILogger _nugetLogger;
    private readonly ICredentialProviderLoaderService _credentialProviderLoaderService;
    private readonly ISourceRepositoryProvider _repositoryProvider;

    public NuGetFeedVerificationService(ICredentialProviderLoaderService credentialProviderLoaderService, 
        ISourceRepositoryProvider repositoryProvider, NuGet.Common.ILogger logger)
    {
        _credentialProviderLoaderService = credentialProviderLoaderService;
        _repositoryProvider = repositoryProvider;
        _nugetLogger = logger;
    }

    public async Task<FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = false, CancellationToken cancellationToken = default)
    {
        Argument.IsNotNullOrEmpty(() => source);

        var result = FeedVerificationResult.Valid;

        var errorMessage = new StringBuilder($"Failed to verify feed '{source}'");

        Logger.LogDebug("Verifying feed '{0}'", source);

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
                throw Logger.LogErrorAndCreateException<OperationCanceledException>("Verification was canceled", ex, cancellationToken);
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
            Logger.LogDebug(ex, errorMessage.ToString());

            result = FeedVerificationResult.Invalid;
        }
        catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
        {
            Logger.LogDebug(ex, errorMessage.ToString());

            result = FeedVerificationResult.Invalid;
        }

        Logger.LogDebug("Verified feed '{0}', result is '{1}'", source, result);

        return result;
    }

    [ObsoleteEx]
    public FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true)
    {
        Argument.IsNotNullOrEmpty(() => source);

        var timeOut = 3000;

        var result = FeedVerificationResult.Valid;

        var errorMessage = new StringBuilder($"Failed to verify feed '{source}'");

        Logger.LogDebug("Verifying feed '{0}'", source);

        try
        {
            var packageSource = new PackageSource(source);

            var repository = _repositoryProvider.CreateRepository(packageSource);

            var searchResource = repository.GetResource<PackageSearchResource>();

            using (var cts = new CancellationTokenSource())
            {
                var cancellationToken = cts.Token;

                //try to perform search
                var searchTask = searchResource.SearchAsync(string.Empty, new SearchFilter(false), 0, 1, _nugetLogger, cancellationToken);

                var searchCompletion = Task.WhenAny(searchTask, Task.Delay(timeOut, cancellationToken)).Result;

                if (searchCompletion != searchTask)
                {
                    throw Logger.LogErrorAndCreateException<TimeoutException>("Search operation has timed out");
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
            Logger.LogDebug(ex, errorMessage.ToString());

            result = FeedVerificationResult.Invalid;
        }
        catch (Exception ex)
        {
            Logger.LogDebug(ex, errorMessage.ToString());

            result = FeedVerificationResult.Invalid;
        }

        Logger.LogDebug("Verified feed '{0}', result is '{1}'", source, result);

        return result;
    }
}
