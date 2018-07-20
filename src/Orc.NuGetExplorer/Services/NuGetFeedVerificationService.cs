// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetFeedVerificationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Linq;
    using System.Net;
    using Catel;
    using Catel.Logging;
    using Catel.Scoping;
    using MethodTimer;
    using NuGet;
    using Scopes;

    internal class NuGetFeedVerificationService : INuGetFeedVerificationService
    {
        #region Fields
        private static readonly ILog Log = LogManager.GetCurrentClassLogger();

        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        #endregion

        #region Constructors
        public NuGetFeedVerificationService(IPackageRepositoryFactory packageRepositoryFactory)
        {
            Argument.IsNotNull(() => packageRepositoryFactory);

            _packageRepositoryFactory = packageRepositoryFactory;
        }
        #endregion

        #region Methods
        [Time]
        public FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true)
        {
            var result = FeedVerificationResult.Valid;

            Log.Debug("Verifying feed '{0}'", source);

            using (ScopeManager<AuthenticationScope>.GetScopeManager(source.GetSafeScopeName(), () => new AuthenticationScope(authenticateIfRequired)))
            {
                try
                {
                    var repository = _packageRepositoryFactory.CreateRepository(source);
                    repository.GetPackages().Take(1).Count();
                }
                catch (WebException ex)
                {
                    result = HandleWebException(ex, source);
                }
                catch (UriFormatException ex)
                {
                    Log.Debug(ex, "Failed to verify feed '{0}', a UriFormatException occurred", source);

                    result = FeedVerificationResult.Invalid;
                }
                catch (Exception ex)
                {
                    Log.Debug(ex, "Failed to verify feed '{0}'", source);

                    result = FeedVerificationResult.Invalid;
                }
            }

            Log.Debug("Verified feed '{0}', result is '{1}'", source, result);

            return result;
        }

        private static FeedVerificationResult HandleWebException(WebException exception, string source)
        {
            try
            {
                var httpWebResponse = (HttpWebResponse)exception.Response;
                if (ReferenceEquals(httpWebResponse, null))
                {
                    return FeedVerificationResult.Invalid;
                }

                if ((int)httpWebResponse.StatusCode == 403)
                {
                    return FeedVerificationResult.Valid;
                }

                if (exception.Status == WebExceptionStatus.ProtocolError)
                {
                    return httpWebResponse.StatusCode == HttpStatusCode.Unauthorized
                        ? FeedVerificationResult.AuthenticationRequired
                        : FeedVerificationResult.Invalid;
                }                
            }
            catch (Exception ex)
            {
                Log.Debug(ex, "Failed to verify feed '{0}'", source);
            }

            return FeedVerificationResult.Invalid;
        }
        #endregion
    }
}
