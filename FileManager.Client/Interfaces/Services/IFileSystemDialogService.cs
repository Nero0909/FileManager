namespace FileManager.Client.Interfaces.Services
{
    public interface IFileSystemDialogService
    {
        string SelectDirectory();

        string SelectFile();
    }
}