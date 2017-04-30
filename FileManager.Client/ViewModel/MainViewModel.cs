using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;
using SystemInterface.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Reactive;
using FileManager.BL.Interfaces.Unity;
using FileManager.BL.Reactive;
using FileManager.BL.Workers;
using FileManager.Client.Interfaces.Model;
using FileManager.Client.Interfaces.Services;
using FileManager.Client.Interfaces.ViewModel;
using FileManager.Client.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.Practices.Unity;
using Reactive.Bindings;

namespace FileManager.Client.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel(
            [Dependency(Dispatchers.Current)] IDispatcherProvider dispatcher,
            IFactory<IManagementViewModel> managementViewModelFactory,
            IFactory<IConfigurationViewModel> configurationViewModelFactory,
            IMessageBoxService messageBoxService,
            IThreadsController threadsController,
            IFile file)
        {
#if DEBUG
            var configurationModel = new ConfigurationModel()
            {
                BufferSize = "10000",
                DestinationFileName = "Test.data",
                DestinationFolder = @"C:\Users\nero0\Desktop\Новая папка (2)",
                SourceFilePath = @"C:\Users\nero0\Downloads\Brainstorm - Years And Seconds.mp3"
            };
#else
            var configurationModel = new ConfigurationModel();
#endif
            ManagementViewModel = managementViewModelFactory.ConstructWith(threadsController).And(configurationModel).Create();
            ConfigurationViewModel = configurationViewModelFactory.ConstructWith<IConfigurationModel>(configurationModel).Create();

            Cancel = threadsController.WriterState.Amb(threadsController.ReaderState)
                .Select(state => state != WorkerState.Unstarted && state != WorkerState.Stopped)
                .ToReactiveCommand(dispatcher.Scheduler, false);
            Cancel.Subscribe(threadsController.Cancel);

            StartCopying = threadsController.WriterState.Amb(threadsController.ReaderState)
                .Select(state => state == WorkerState.Unstarted || state == WorkerState.Stopped)
                .CombineLatest(ConfigurationViewModel.HasNotErrors, (state, err) => state && err)
                .ToReactiveCommand(dispatcher.Scheduler);

            StartCopying.Subscribe(() =>
            {
                var destPath = Path.Combine(configurationModel.DestinationFolder, configurationModel.DestinationFileName);
                var bufferSize = int.Parse(configurationModel.BufferSize);
                if (file.Exists(destPath))
                {
                    if (
                        messageBoxService.ShowYesNowQuestion(
                            $"File {destPath} already exists. Would you like to replace it?", "Information") ==
                        MessageBoxResult.Yes)
                    {
                        threadsController.StartReadAndWrite(configurationModel.SourceFilePath, destPath, bufferSize);
                    }
                }
                else
                {
                    threadsController.StartReadAndWrite(configurationModel.SourceFilePath, destPath, bufferSize);
                }
            });
        }

        public IManagementViewModel ManagementViewModel { get; }

        public IConfigurationViewModel ConfigurationViewModel { get; }

        public ReactiveCommand StartCopying { get; }

        public ReactiveCommand Cancel { get; }
    }
}