// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeedVerificationResult.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public enum FeedVerificationResult
    {
        Unknown,
        Valid,
        AuthenticationRequired,
        AuthorizationRequired, // what is different from  AuthenticationRequired?
        Invalid
    }
}
