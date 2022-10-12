namespace Orc.NuGetExplorer.Providers
{
    using System.ComponentModel;
    using Catel;
    using Catel.Data;
    using Catel.IoC;

    public class ModelProvider<T> : IModelProvider<T> where T : ModelBase
    {
        private readonly ITypeFactory _typeFactory;

        public ModelProvider(ITypeFactory typeFactory)
        {
            Argument.IsNotNull(() => typeFactory);

            _typeFactory = typeFactory;
        }

        private T? _model;

        public virtual T? Model
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

        public event PropertyChangedEventHandler? PropertyChanged;

        public virtual T Create()
        {
            return _typeFactory.CreateRequiredInstance<T>();
        }

        private void RaisePropertyChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model)));
        }
    }
}
