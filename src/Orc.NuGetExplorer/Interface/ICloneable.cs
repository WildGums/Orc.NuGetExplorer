namespace Orc.NuGetExplorer;

public interface ICloneable<out T>
{
    T Clone();
}