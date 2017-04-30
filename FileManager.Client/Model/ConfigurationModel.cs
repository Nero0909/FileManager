using FileManager.Client.Interfaces.Model;
using GalaSoft.MvvmLight;

namespace FileManager.Client.Model
{
    public class ConfigurationModel : ObservableObject, IConfigurationModel
    {
        private string _sourceFilePath;
        private string _destinationFolder;
        private string _destinationFileName;
        private string _bufferSize;

        public string SourceFilePath
        {
            get { return _sourceFilePath;}
            set { Set(() => SourceFilePath, ref _sourceFilePath, value); }
        }

        public string DestinationFolder
        {
            get { return _destinationFolder; }
            set { Set(() => DestinationFolder, ref _destinationFolder, value); }
        }

        public string DestinationFileName
        {
            get { return _destinationFileName; }
            set { Set(() => DestinationFileName, ref _destinationFileName, value); }
        }

        public string BufferSize
        {
            get { return _bufferSize; }
            set { Set(() => BufferSize, ref _bufferSize, value); }
        }
    }
}
