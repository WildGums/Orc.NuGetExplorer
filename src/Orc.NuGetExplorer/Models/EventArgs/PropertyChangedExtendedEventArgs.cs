namespace Orc.NuGetExplorer;

using System.ComponentModel;
using System.Runtime.CompilerServices;

public class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
{
    public virtual T? OldValue { get; }
    public virtual T? NewValue { get; }

    public PropertyChangedExtendedEventArgs(T? oldValue, T? newValue, [CallerMemberName] string propertyName = "")
        : base(propertyName)
    {
        OldValue = oldValue;
        NewValue = newValue;
    }
}
