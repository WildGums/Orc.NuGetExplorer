// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFileSystemService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IFileSystemService
    {
        #region Methods
        void CreateDeleteme(string name, string path);
        void RemoveDeleteme(string name, string path);
        #endregion
    }
}
