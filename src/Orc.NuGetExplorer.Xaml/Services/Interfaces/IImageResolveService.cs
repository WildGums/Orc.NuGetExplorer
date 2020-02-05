// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageResolveService.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public interface IImageResolveService
    {
        #region Methods
        ImageSource ResolveImageFromUri(Uri uri, string defaultUrl = null);
        Task<ImageSource> ResolveImageFromUriAsync(Uri uri, string defaultUrl = null);
        #endregion
    }
}
