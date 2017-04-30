using System.Windows;
using FileManager.Client.Interfaces.Services;

namespace FileManager.Client.Services
{
    public class MessageBoxService : IMessageBoxService
    {
        public MessageBoxResult ShowInformation(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public MessageBoxResult ShowErrorMessage(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public MessageBoxResult ShowYesNowQuestion(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButton.YesNoCancel, MessageBoxImage.Information);
        }
    }
}
