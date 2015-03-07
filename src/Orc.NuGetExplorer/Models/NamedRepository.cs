// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamedRepository.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using Catel.Data;
    using NuGet;

    internal class NamedRepository : ModelBase
    {
        #region Constructors
        public NamedRepository()
        {
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public IPackageRepository Value { get; set; }
        public RepositoryCategoryType RepositoryCategory { get; set; }
        #endregion
    }
}