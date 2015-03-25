// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchSettings.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Orc.NuGetExplorer
{
    using Catel.Data;

    public class SearchSettings : ModelBase
    {
        public bool? IsPrereleaseAllowed { get; set; }
        public string SearchFilter { get; set; }
        public string FilterWatermark { get; set; }
    }
}