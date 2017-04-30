using FileManager.BL.Workers;
using Reactive.Bindings;

namespace FileManager.Client.Interfaces.ViewModel
{
    public interface IManagementViewModel
    {
        ReactiveProperty<WorkerState> ReaderState { get; }

        ReactiveProperty<WorkerState> WriterState { get; }

        ReactiveProperty<double> BufferSize { get; }

        ReactiveProperty<double> Progress { get; }

        ReactiveCommand PauseReading { get; }

        ReactiveCommand ResumeReading { get; }

        ReactiveCommand PauseWriting { get; }

        ReactiveCommand ResumeWriting { get; }
    }
}