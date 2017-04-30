using System.Windows;

namespace FileManager.Client.Interfaces.Services
{
    public interface IMessageBoxService
    {
        MessageBoxResult ShowInformation(string text, string caption);

        MessageBoxResult ShowErrorMessage(string text, string caption);

        MessageBoxResult ShowYesNowQuestion(string text, string caption);
    }
}