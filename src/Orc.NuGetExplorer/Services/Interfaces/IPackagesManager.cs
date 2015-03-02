// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackagesManager.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System.Runtime.Versioning;
    using System.Threading.Tasks;
    using NuGet;

    public interface IPackagesManager
    {
        #region Methods
        Task Show();

        Task Install(IPackage package, IPackageRepository packageRepository, FrameworkName targetFramework = null);
        #endregion
    }
}