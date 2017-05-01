using System;
using System.Reactive.Subjects;
using FileManager.BL.Workers;

namespace FileManager.BL.Interfaces
{
    public interface ISuspendableWorker
    {
        void Pause();

        void Resume();

        void Cancel();

        IConnectableObservable<int> DoWork(string filePath);

        IObservable<WorkerState> CurrentState { get; }

        IObservable<ResultDto> Result { get; }
    }
}