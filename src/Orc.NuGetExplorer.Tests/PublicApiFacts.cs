// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PublicApiFacts.cs" company="WildGums">
//   Copyright (c) 2008 - 2017 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Tests
{
    using System.Runtime.CompilerServices;
    using ApiApprover;
    using NUnit.Framework;

    [TestFixture]
    public class PublicApiFacts
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Orc_NuGetExplorer_HasNoBreakingChanges()
        {
            var assembly = typeof(PackageCacheService).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }

        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public void Orc_NuGetExplorer_Xaml_HasNoBreakingChanges()
        {
            var assembly = typeof(XamlPleaseWaitInterruptService).Assembly;

            PublicApiApprover.ApprovePublicApi(assembly);
        }
    }
}