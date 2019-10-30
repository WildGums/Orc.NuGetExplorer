namespace Orc.NuGetExplorer
{
    using System.Threading.Tasks;
    using NuGetExplorer.Windows;

    public interface IMessageDialogService
    {
        Task<T> ShowDialogAsync<T>(string title, string message, bool addCloseButton, params IDialogOption[] options);

        T ShowDialog<T>(string title, string message, bool addCloseButton, params IDialogOption[] options);
    }
}
