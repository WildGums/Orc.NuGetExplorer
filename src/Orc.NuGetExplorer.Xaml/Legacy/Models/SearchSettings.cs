// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchSettings.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using Catel.Data;

    internal class SearchSettings : ModelBase
    {
        public bool? IsPrereleaseAllowed { get; set; }
        public string SearchFilter { get; set; }
        public int PackagesToSkip { get; set; }
    }
}