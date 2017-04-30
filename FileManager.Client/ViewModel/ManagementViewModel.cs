using System;
using System.Reactive.Linq;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Reactive;
using FileManager.BL.Reactive;
using FileManager.BL.Workers;
using FileManager.Client.Interfaces.Services;
using FileManager.Client.Interfaces.ViewModel;
using FileManager.Client.Model;
using Microsoft.Practices.Unity;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace FileManager.Client.ViewModel
{
    public class ManagementViewModel : IManagementViewModel
    {
        public ManagementViewModel([Dependency(Dispatchers.Current)] IDispatcherProvider dispatcher,
            IMessageBoxService messageBoxService,
            IThreadsController threadsController)
        {
            threadsController.Result.Subscribe(r =>
            {
                if (r.Result == Result.Error)
                {
                    messageBoxService.ShowErrorMessage(r.Exception.ToString(), "Error");
                }
                else
                {
                    messageBoxService.ShowInformation(r.Result.ToString(), "Information");
                }
            });

            Progress =
                threadsController.Progress
                    .Sample(TimeSpan.FromMilliseconds(50))
                    .ToReactiveProperty(dispatcher.Scheduler);

            BufferSize =
                threadsController.CurrentBufferSize
                    .Sample(TimeSpan.FromMilliseconds(50))
                    .ToReactiveProperty(dispatcher.Scheduler);

            ReaderState = threadsController.ReaderState.ToReactiveProperty(dispatcher.Scheduler);
            WriterState = threadsController.WriterState.ToReactiveProperty(dispatcher.Scheduler);

            PauseReading = threadsController.ReaderState.Select(x => x == WorkerState.Running)
                .ToReactiveCommand(dispatcher.Scheduler);
            PauseReading.Subscribe(threadsController.PauseReading);

            ResumeReading = threadsController.ReaderState.Select(x => x == WorkerState.Suspended)
                .ToReactiveCommand(dispatcher.Scheduler);
            ResumeReading.Subscribe(threadsController.ResumeReading);


            PauseWriting = threadsController.WriterState.Select(x => x == WorkerState.Running)
                .ToReactiveCommand(dispatcher.Scheduler);
            PauseWriting.Subscribe(threadsController.PauseWriting);

            ResumeWriting = threadsController.WriterState.Select(x => x == WorkerState.Suspended)
                .ToReactiveCommand(dispatcher.Scheduler);
            ResumeWriting.Subscribe(threadsController.ResumeWriting);
        }

        public ReactiveProperty<WorkerState> ReaderState { get; }

        public ReactiveProperty<WorkerState> WriterState { get; }

        public ReactiveProperty<double> BufferSize { get; }

        public ReactiveProperty<double> Progress { get; }
        
        public ReactiveCommand PauseReading { get; }

        public ReactiveCommand ResumeReading { get; }

        public ReactiveCommand PauseWriting { get; }

        public ReactiveCommand ResumeWriting { get; }
    }
}