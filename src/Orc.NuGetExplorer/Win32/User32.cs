namespace Orc.NuGetExplorer.Win32
{
    using System;
    using System.Runtime.InteropServices;

    internal static class User32
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetActiveWindow();
    }
}
