// --------------------------------------------------------------------------------------------------------------------
// <copyright file="User32.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.NuGetExplorer.Native
{
    using System;
    using System.Runtime.InteropServices;

    public static class User32
    {
        #region Methods
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        #endregion
    }
}