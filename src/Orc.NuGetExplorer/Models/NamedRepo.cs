// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedRepo.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Data;
    using NuGet;

    public class NamedRepo : ModelBase
    {
        #region Properties
        public string Name { get; set; }
        public IPackageRepository Value { get; set; }
        #endregion
    }
}