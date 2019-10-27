namespace Orc.NuGetExplorer.Services
{
    using NuGetExplorer.Windows;
    using System.Threading.Tasks;

    public interface IMessageDialogService
    {
        Task<T> ShowDialogAsync<T>(string title, string message, bool addCloseButton, params IDialogOption[] options);

        T ShowDialog<T>(string title, string message, bool addCloseButton, params IDialogOption[] options);
    }
}
