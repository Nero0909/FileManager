using System.ComponentModel;

namespace FileManager.Client.Interfaces.Model
{
    public interface IConfigurationModel : INotifyPropertyChanged
    {
        string SourceFilePath { get; set; }

        string DestinationFolder { get; set; }

        string DestinationFileName { get; set; }

        string BufferSize { get; set; }
    }
}