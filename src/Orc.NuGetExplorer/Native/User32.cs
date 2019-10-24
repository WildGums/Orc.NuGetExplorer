// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User32.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.InteropServices;

    internal static class User32
    {
        #region Methods
        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();
        #endregion
    }
}
