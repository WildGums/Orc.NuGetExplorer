namespace Orc.NuGetExplorer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Catel;
    using Catel.Logging;
    using NuGet.Common;
    using NuGet.Configuration;
    using NuGet.Credentials;

    public class ExplorerCredentialService : ICredentialService
    {
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly ConcurrentDictionary<string, bool> _retryCache
             = new ConcurrentDictionary<string, bool>();

        private readonly ConcurrentDictionary<string, CredentialResponse> _providerCredentialCache
            = new ConcurrentDictionary<string, CredentialResponse>();

        private readonly bool _nonInteractive;

        /// <summary>
        /// Gets the currently configured providers.
        /// </summary>
        private AsyncLazy<IEnumerable<ICredentialProvider>> _providers { get; }

        private readonly Semaphore _providerSemaphore = new Semaphore(1, 1);

        public bool HandlesDefaultCredentials { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="providers">All available credential providers.</param>
        /// <param name="nonInteractive">If true, the nonInteractive flag will be passed to providers.
        /// <param name="handlesDefaultCredentials"> If true, specifies that this credential service handles default credentials as well.
        /// That means that DefaultNetworkCredentialsCredentialProvider instance is in the list of providers. It's set explicitly as a perfomance optimization.</param>
        /// NonInteractive requests must not promt the user for credentials.</param>
        public ExplorerCredentialService(AsyncLazy<IEnumerable<ICredentialProvider>> providers, bool nonInteractive, bool handlesDefaultCredentials)
        {
            Argument.IsNotNull(() => providers);

            _providers = providers;
            _nonInteractive = nonInteractive;
            HandlesDefaultCredentials = handlesDefaultCredentials;
        }

        /// <summary>
        /// Provides credentials for http requests.
        /// </summary>
        /// <param name="uri">
        /// The URI of a web resource for which credentials are needed.
        /// </param>
        /// <param name="proxy">
        /// The currently configured proxy. It may be necessary for CredentialProviders
        /// to use this proxy in order to acquire credentials from their authentication source.
        /// </param>
        /// <param name="type">
        /// The type of credential request that is being made.
        /// </param>
        /// <param name="message">
        /// A default, user-readable message explaining why they are being prompted for credentials.
        /// The credential provider can choose to ignore this value and write their own message.
        /// </param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A credential object, or null if no credentials could be acquired.</returns>
        public async Task<ICredentials> GetCredentialsAsync(
            Uri uri,
            IWebProxy proxy,
            CredentialRequestType type,
            string message,
            CancellationToken cancellationToken)
        {
            if (uri is null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            ICredentials creds = null;

            foreach (var provider in await _providers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var retryKey = CredentialsKeyHelper.GetUriKey(uri, type, provider);
                var isRetry = _retryCache.ContainsKey(retryKey);

                try
                {
                    //original implementation contains semaphore
                    //in fact unecessary, because service called
                    //only when no one provider cached

                    _providerSemaphore.WaitOne();

                    Log.Debug($"Requesting credentials, _retryCache count = {_retryCache.Count}");

                    if (!TryFromCredentialCache(uri, type, isRetry, provider, out var response))
                    {
                        response = await provider.GetAsync(
                            uri,
                            proxy,
                            type,
                            message,
                            isRetry,
                            _nonInteractive,
                            cancellationToken);

                        // Check that the provider gave us a valid response.
                        if (!IsValidResponse(response))
                        {
                            throw new ProviderException("Credential provider gaves malformed response.");
                        }

                        if (response.Status == CredentialStatus.UserCanceled)
                        {
                            //create cancellation
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                        else
                        {
                            AddToCredentialCache(uri, type, provider, response);
                        }
                    }

                    if (response.Status == CredentialStatus.Success)
                    {
                        _retryCache[retryKey] = true;
                        Log.Debug($"_retryCache count now is {_retryCache.Count}");
                        creds = response.Credentials;
                        break;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                    _providerSemaphore.Release();
                }
            }

            return creds;
        }

        public bool IsValidResponse(CredentialResponse response)
        {
            return response != null && Enum.IsDefined(typeof(CredentialStatus), response.Status);
        }

        /// <summary>
        /// Attempts to retrieve last known good credentials for a URI from a credentials cache.
        /// </summary>
        /// <remarks>
        /// When the return value is <c>true</c>, <paramref name="credentials" /> will have last known
        /// good credentials from the credentials cache.  These credentials may have become invalid
        /// since their last use, so there is no guarantee that the credentials are currently valid.
        /// </remarks>
        /// <param name="uri">The URI for which cached credentials should be retrieved.</param>
        /// <param name="isProxy"><c>true</c> for proxy credentials; otherwise, <c>false</c>.</param>
        /// <param name="credentials">Cached credentials or <c>null</c>.</param>
        /// <returns><c>true</c> if a result is returned from the cache; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri" /> is <c>null</c>.</exception>
        public bool TryGetLastKnownGoodCredentialsFromCache(
            Uri uri,
            bool isProxy,
            out ICredentials credentials)
        {
            if (uri == null)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            credentials = null;

            var rootUri = uri.GetRootUri();
            var ending = $"_{isProxy}_{rootUri}";

            foreach (var entry in _providerCredentialCache)
            {
                if (entry.Value.Status == CredentialStatus.Success && entry.Key.EndsWith(ending))
                {
                    credentials = entry.Value.Credentials;

                    return true;
                }
            }

            return false;
        }

        private bool TryFromCredentialCache(Uri uri, CredentialRequestType type, bool isRetry, ICredentialProvider provider,
            out CredentialResponse credentials)
        {
            credentials = null;

            var key = CredentialsKeyHelper.GetCacheKey(uri, type, provider);

            if (isRetry)
            {
                CredentialResponse removed;
                _providerCredentialCache.TryRemove(key, out removed);
                return false;
            }

            return _providerCredentialCache.TryGetValue(key, out credentials);
        }

        private void AddToCredentialCache(Uri uri, CredentialRequestType type, ICredentialProvider provider,
            CredentialResponse credentials)
        {
            _providerCredentialCache[CredentialsKeyHelper.GetCacheKey(uri, type, provider)] = credentials;
        }


        /// <summary>
        /// Clear retry cache. As long as we don't recreate SourceRepository instances this is unnecessary
        /// </summary>
        public void ClearRetryCache()
        {
            _retryCache.Clear();
            Log.Debug($"_retryCache count {_retryCache.Count}");
        }

        internal static class CredentialsKeyHelper
        {
            public static string GetCacheKey(Uri uri, CredentialRequestType type, ICredentialProvider provider)
            {
                // Note: don't cache by root uri, just remove catalog info

                var rootUri = uri.GetRootUri();
                return GetUriKey(rootUri, type, provider);
            }

            public static string GetUriKey(Uri uri, CredentialRequestType type, ICredentialProvider provider)
            {
                return $"{provider.Id}_{type == CredentialRequestType.Proxy}_{uri}";
            }
        }
    }
}
