// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationServiceExtensions.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using Catel;
    using Catel.Threading;

    public static class INuGetFeedVerificationServiceExtensions
    {
        #region Methods
        public static Task<FeedVerificationResult> VerifyFeedAsync(this INuGetFeedVerificationService nuGetFeedVerificationService,
            string source, bool authenticateIfRequired = true)
        {
            Argument.IsNotNull(() => nuGetFeedVerificationService);

            return TaskHelper.Run(() => nuGetFeedVerificationService.VerifyFeed(source, authenticateIfRequired));
        }
        #endregion
    }
}