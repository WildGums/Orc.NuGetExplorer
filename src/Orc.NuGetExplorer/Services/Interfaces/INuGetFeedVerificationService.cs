// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INuGetFeedVerificationService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orc.NuGetExplorer
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface INuGetFeedVerificationService
    {
        #region Methods
        Task<FeedVerificationResult> VerifyFeedAsync(string source, bool authenticateIfRequired = true, CancellationToken cancellationToken = default);
        FeedVerificationResult VerifyFeed(string source, bool authenticateIfRequired = true);
        #endregion
    }
}
