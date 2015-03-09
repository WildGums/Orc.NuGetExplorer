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
        #endregion

        #region Constructors
        public NuGetFeedVerificationService(IPackageRepositoryFactory packageRepositoryFactory)
        {
            Argument.IsNotNull(() => packageRepositoryFactory);

            _packageRepositoryFactory = packageRepositoryFactory;
        }
        #endregion

        #region Methods
        public FeedVerificationResult VerifyFeed(string source)
        {
            var result = FeedVerificationResult.Valid;
            try
            {
                var repository = _packageRepositoryFactory.CreateRepository(source);
                var packages = repository.GetPackages().Take(1).ToArray();
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = ex.Response as HttpWebResponse;
                    if (response != null && response.StatusCode == HttpStatusCode.Unauthorized)
                    {
                        result = FeedVerificationResult.AuthenticaitonRequired;
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

            return result;
        }
        #endregion
    }
}