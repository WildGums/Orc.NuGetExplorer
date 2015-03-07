// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPackageSource.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IPackageSource
    {
        #region Properties
        bool IsEnabled { get; set; }
        bool IsMachineWide { get; set; }
        bool IsOfficial { get; set; }
        bool IsPasswordClearText { get; set; }
        string Name { get; }
        string Password { get; set; }
        string Source { get; }
        string UserName { get; set; }
        #endregion
    }
}