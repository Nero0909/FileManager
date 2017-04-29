using System;
using FileManager.BL.Workers;

namespace FileManager.BL.Interfaces
{
    public interface ISuspendable
    {
        void Pause();

        void Resume();

        void Cancel();

        IObservable<WorkerState> CurrentState { get; }
    }
}