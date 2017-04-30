using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Input;
using SystemInterface.IO;
using FileManager.Client.Interfaces.Model;
using FileManager.Client.Interfaces.Services;
using FileManager.Client.Interfaces.ViewModel;
using FileManager.Client.Model;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FileManager.Client.ViewModel
{
    public class ConfigurationViewModel : IConfigurationViewModel
    {
        public IConfigurationModel Model { get; }

        public ReactiveProperty<string> SourceFilePath { get; }

        public ReactiveProperty<string> DestinationFolder { get; }

        public ReactiveProperty<string> DestinationFileName { get; }

        public ReactiveProperty<string> BufferSize { get; }

        public IObservable<bool> HasNotErrors { get; }

        public ICommand SelectSourceFileCommand { get; }

        public ICommand SelectDestinationFolderCommand { get; }

        public ConfigurationViewModel(
            IFile file,
            IDirectory directory,
            IFileSystemDialogService dialogService,
            IConfigurationModel model)
        {
            Model = model;

            SourceFilePath = Model
                .ToReactivePropertyAsSynchronized(x => x.SourceFilePath)
                .SetValidateNotifyError(x => !string.IsNullOrEmpty(x) && file.Exists(x) ? null : "The file doesn't exist.");

            DestinationFolder = Model
                .ToReactivePropertyAsSynchronized(x => x.DestinationFolder)
                .SetValidateNotifyError(x => !string.IsNullOrEmpty(x) && directory.Exists(x) ? null : "The directory doesn't exist.");

            DestinationFileName = Model
                .ToReactivePropertyAsSynchronized(x => x.DestinationFileName)
                .SetValidateNotifyError(x => !string.IsNullOrEmpty(x) ? null : "The file name mustn't be empty.");

            BufferSize = Model
                .ToReactivePropertyAsSynchronized(x => x.BufferSize)
                .SetValidateNotifyError(x =>
                {
                    var isPositiveInteger = new Regex(@"^[1-9]\d*$");
                    return !string.IsNullOrEmpty(x) && isPositiveInteger.IsMatch(x) ? null : "The buffer size have to be a positive integer value.";
                });

            HasNotErrors = new[]
                {
                    SourceFilePath.ObserveHasErrors,
                    DestinationFileName.ObserveHasErrors,
                    DestinationFolder.ObserveHasErrors,
                    BufferSize.ObserveHasErrors
                }
                .CombineLatestValuesAreAllFalse();

            SelectSourceFileCommand = new RelayCommand(() =>
            {
                var path = dialogService.SelectFile();
                if (!string.IsNullOrEmpty(path))
                {
                    Model.SourceFilePath = path;
                }
            });

            SelectDestinationFolderCommand = new RelayCommand(() =>
            {
                var path = dialogService.SelectDirectory();
                if (!string.IsNullOrEmpty(path))
                {
                    Model.DestinationFolder = path;
                }
            });
        }
    }
}
