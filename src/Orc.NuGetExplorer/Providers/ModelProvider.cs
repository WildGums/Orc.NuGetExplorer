namespace Orc.NuGetExplorer.Providers
{
    using System.ComponentModel;
    using Catel.Data;

    public class ModelProvider<T> : IModelProvider<T> where T : ModelBase
    {
        private T _model;

        public virtual T Model
        {
            get => _model;
            set
            {
                if (value != _model)
                {
                    _model = value;
                    RaisePropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model)));
        }
    }
}
