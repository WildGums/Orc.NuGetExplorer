// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetFeedVerificationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Linq;
    using System.Net;
    using Catel;
    using NuGet;

    internal class NuGetFeedVerificationService : INuGetFeedVerificationService
    {
        #region Fields
        private readonly IAuthenticationSilencerService _authenticationSilencerService;
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        #endregion

        #region Constructors
        public NuGetFeedVerificationService(IPackageRepositoryFactory packageRepositoryFactory, IAuthenticationSilencerService authenticationSilencerService)
        {
            Argument.IsNotNull(() => packageRepositoryFactory);
            Argument.IsNotNull(() => authenticationSilencerService);

            _packageRepositoryFactory = packageRepositoryFactory;
            _authenticationSilencerService = authenticationSilencerService;
        }
        #endregion

        #region Methods
        public FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true)
        {
            var result = FeedVerificationResult.Valid;

            using (_authenticationSilencerService.AuthenticationRequiredScope(authenticateIfRequired))
            {
                try
                {
                    var repository = _packageRepositoryFactory.CreateRepository(source);
                    var packagesCount = repository.GetPackages().Take(1).Count();
                }
                catch (WebException ex)
                {
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var response = ex.Response as HttpWebResponse;
                        if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            result = FeedVerificationResult.AuthenticationRequired;
                        }
                        else
                        {
                            result = FeedVerificationResult.Invalid;
                        }
                    }
                    else
                    {
                        result = FeedVerificationResult.Invalid;
                    }
                }
                catch
                {
                    result = FeedVerificationResult.Invalid;
                }
            }
            return result;
        }
        #endregion
    }
}