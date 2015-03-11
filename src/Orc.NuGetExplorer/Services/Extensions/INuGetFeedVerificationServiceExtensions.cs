// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;

    public static class AuthenticationRequiredExtensions
    {
        #region Methods
        public static async Task<FeedVerificationResult> VerifyFeedAsync(this INuGetFeedVerificationService nuGetFeedVerificationService, 
            string source, bool authenticateIfRequired = true)
        {
            Argument.IsNotNull(() => nuGetFeedVerificationService);

            return await Task.Factory.StartNew(() => nuGetFeedVerificationService.VerifyFeed(source, authenticateIfRequired));
        }
        #endregion
    }
}