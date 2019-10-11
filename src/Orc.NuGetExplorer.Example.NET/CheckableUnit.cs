namespace Orc.NuGetExplorer
{
    using Catel.Data;
    using System;

    public class CheckableUnit<T> : ObservableObject
    {
        private readonly Action<bool, T> _onCheckedChangedCallback;

        public CheckableUnit(bool isChecked, T value)
        {
            IsChecked = isChecked;
            Value = value;
        }

        public CheckableUnit(bool isChecked, T value, Action<bool, T> onCheckedChangedCallback)
            : this(isChecked, value)
        {
            _onCheckedChangedCallback = onCheckedChangedCallback;
        }

        public bool IsChecked { get; set; }

        protected override void OnPropertyChanged(AdvancedPropertyChangedEventArgs e)
        {
            if (Value == null)
            {
                return;
            }

            if (string.Equals(e.PropertyName, nameof(IsChecked)))
            {
                _onCheckedChangedCallback((bool)e.NewValue, Value);
            }
        }

        public T Value { get; set; }
    }
}
