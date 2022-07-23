namespace Orc.NuGetExplorer
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Media;

    public interface IImageResolveService
    {
        Task<ImageSource> ResolveImageFromUriAsync(Uri uri, string defaultUrl = null);
    }
}
