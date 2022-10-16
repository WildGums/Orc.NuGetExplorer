namespace Orc.NuGetExplorer
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    internal class PropertyChangedExtendedEventArgs<T> : PropertyChangedEventArgs
    {
        public virtual T OldValue { get; private set; }
        public virtual T NewValue { get; private set; }

        public PropertyChangedExtendedEventArgs(T oldValue, T newValue, [CallerMemberName] string propertyName = "")
               : base(propertyName)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
