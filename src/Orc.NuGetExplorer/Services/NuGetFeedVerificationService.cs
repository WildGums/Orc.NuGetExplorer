// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NuGetFeedVerificationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Linq;
    using System.Net;
    using Catel;
    using NuGet;

    internal class NuGetFeedVerificationService : INuGetFeedVerificationService
    {
        #region Fields
        private readonly IPackageRepositoryFactory _packageRepositoryFactory;
        private readonly ICredentialProvider _credentialProvider;
        #endregion

        #region Constructors
        public NuGetFeedVerificationService(IPackageRepositoryFactory packageRepositoryFactory, ICredentialProvider credentialProvider)
        {
            Argument.IsNotNull(() => packageRepositoryFactory);
            Argument.IsNotNull(() => credentialProvider);

            _packageRepositoryFactory = packageRepositoryFactory;
            _credentialProvider = credentialProvider;
        }
        #endregion

        #region Methods
        public FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true)
        {
            var result = FeedVerificationResult.Valid;
            var originalCredentialProvider = HttpClient.DefaultCredentialProvider;
            try
            {
                if (!authenticateIfRequired)
                {
                    HttpClient.DefaultCredentialProvider = NullCredentialProvider.Instance;
                }

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
            catch (Exception)
            {
                result = FeedVerificationResult.Invalid;
            }
            finally
            {
                if (!authenticateIfRequired)
                {
                    HttpClient.DefaultCredentialProvider = originalCredentialProvider;
                }
            }

            return result;
        }
        #endregion
    }
}