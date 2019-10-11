namespace Orc.NuGetExplorer.Providers
{
    using Catel.Data;
    using System.ComponentModel;

    public interface IModelProvider<T> : INotifyPropertyChanged where T : ModelBase
    {
        T Model { get; set; }
    }
}
