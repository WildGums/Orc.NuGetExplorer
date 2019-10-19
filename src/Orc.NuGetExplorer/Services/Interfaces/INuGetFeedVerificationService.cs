// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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