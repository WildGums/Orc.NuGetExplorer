namespace Orc.NuGetExplorer;

using System;
using System.Threading.Tasks;
using System.Windows.Media;

public interface IImageResolveService
{
    ImageSource ResolveImageFromUri(Uri uri);
    Task<ImageSource?> ResolveImageFromUriAsync(Uri uri);
}