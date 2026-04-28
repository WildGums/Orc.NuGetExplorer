namespace Orc.NuGetExplorer.Providers;

using System;
using System.ComponentModel;
using Catel.Data;
using Catel.IoC;
using Microsoft.Extensions.DependencyInjection;

public class ModelProvider<T> : IModelProvider<T> 
    where T : ModelBase
{
    private readonly IServiceProvider _serviceProvider;

    public ModelProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
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
        return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
    }

    private void RaisePropertyChanged()
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Model)));
    }
}
