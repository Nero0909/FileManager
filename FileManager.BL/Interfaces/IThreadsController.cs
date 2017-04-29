using System;
using FileManager.BL.Workers;

namespace FileManager.BL.Interfaces
{
    public interface IThreadsController
    {
        void Cancel();

        void PauseWriting();

        void ResumeWriting();

        void PauseReading();

        void ResumeReading();

        IObservable<int> CurrentBufferSize { get; }

        IObservable<double> Progress { get; }

        IObservable<WorkerState> ReaderState { get; }

        IObservable<WorkerState> WriterState { get; }
    }
}