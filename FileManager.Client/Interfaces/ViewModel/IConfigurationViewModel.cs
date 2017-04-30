using System;
using System.Windows.Input;
using FileManager.Client.Interfaces.Model;
using Reactive.Bindings;

namespace FileManager.Client.Interfaces.ViewModel
{
    public interface IConfigurationViewModel
    {
        IConfigurationModel Model { get; }

        ReactiveProperty<string> SourceFilePath { get; }

        ReactiveProperty<string> DestinationFolder { get; }

        ReactiveProperty<string> DestinationFileName { get; }

        ReactiveProperty<string> BufferSize { get; }

        IObservable<bool> HasNotErrors { get; }

        ICommand SelectSourceFileCommand { get; }

        ICommand SelectDestinationFolderCommand { get; }
    }
}