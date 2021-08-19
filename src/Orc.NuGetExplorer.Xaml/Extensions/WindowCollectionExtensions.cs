namespace Orc.NuGetExplorer
{
    using System.Linq;
    using System.Windows;
    using Catel.Windows;

    public static class WindowCollectionExtensions
    {
        public static DataWindow GetCurrentActiveDataWindow(this WindowCollection windows)
        {
            return windows.OfType<DataWindow>().FirstOrDefault(x => x.IsActive);
        }
    }
}
