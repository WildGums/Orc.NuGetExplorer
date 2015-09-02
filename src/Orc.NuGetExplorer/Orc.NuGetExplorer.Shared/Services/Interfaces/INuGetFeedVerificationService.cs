// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface INuGetFeedVerificationService
    {
        #region Methods
        FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true);
        #endregion
    }
}