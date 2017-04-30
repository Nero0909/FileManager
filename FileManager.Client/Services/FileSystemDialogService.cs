using System.Windows;
using System.Windows.Forms;
using FileManager.Client.Interfaces.Services;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace FileManager.Client.Services
{
    public class FileSystemDialogService : IFileSystemDialogService
    {
        public string SelectDirectory()
        {
            using (var dialog = new FolderBrowserDialog() { ShowNewFolderButton = true })
            {
                var selectedPath = string.Empty;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    selectedPath = dialog.SelectedPath;
                }
                return selectedPath;
            }
        }

        public string SelectFile()
        {
            var fileName = string.Empty;
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                fileName = dialog.FileName;
            }

            return fileName;
        }
    }
}
