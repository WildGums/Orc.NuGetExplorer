namespace Orc.NuGetExplorer.Providers
{
    using System.ComponentModel;
    using Catel.Data;

    public interface IModelProvider<T> : INotifyPropertyChanged where T : ModelBase
    {
        T? Model { get; set; }

        T Create();
    }
}
