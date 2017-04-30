using System;
using FileManager.BL.Workers;

namespace FileManager.BL.Interfaces
{
    public interface ISuspendableWorker
    {
        void Pause();

        void Resume();

        void Cancel();

        IObservable<WorkerState> CurrentState { get; }

        IObservable<ResultDto> Result { get; }
    }
}