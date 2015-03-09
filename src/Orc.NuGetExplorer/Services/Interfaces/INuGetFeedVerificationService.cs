// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface INuGetFeedVerificationService
    {
        FeedVerificationResult VerifyFeed(string source);
    }
}