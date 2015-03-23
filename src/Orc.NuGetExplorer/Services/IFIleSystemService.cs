// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFIleSystemService.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    public interface IFIleSystemService
    {
        void DeleteDirectory(string path);
        void CopyDirectory(string sourceDirectory, string destinationDirectory);
    }
}