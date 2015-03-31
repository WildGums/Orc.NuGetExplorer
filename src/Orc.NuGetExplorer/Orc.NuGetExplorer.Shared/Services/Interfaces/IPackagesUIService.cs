// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesUIService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;

    public interface IPackagesUIService
    {
        #region Methods
        Task ShowPackagesExplorer();
        #endregion
    }
}