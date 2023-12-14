namespace Orc.NuGetExplorer;

using System;
using System.ComponentModel;
using Catel.Data;

internal class CheckableUnit<T> : ObservableObject
{
    private readonly Action<T, bool> _onCheckedChangedCallback;

    public CheckableUnit(bool isChecked, T value, Action<T, bool> onCheckedChangedCallback)
    {
        IsChecked = isChecked;
        Value = value;

        _onCheckedChangedCallback = onCheckedChangedCallback;
    }

    public bool IsChecked { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        if (Value is null)
        {
            return;
        }

        if (e.HasPropertyChanged(nameof(IsChecked)))
        {
            _onCheckedChangedCallback(Value, IsChecked);
        }
    }

    public T Value { get; set; }
}