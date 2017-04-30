using System;
using FileManager.BL.Workers;

namespace FileManager.BL.Interfaces
{
    public interface IThreadsController
    {
        void StartReadAndWrite(string srcPath, string destPath, int bufferSize);

        void Cancel();

        void PauseWriting();

        void ResumeWriting();

        void PauseReading();

        void ResumeReading();

        IObservable<double> CurrentBufferSize { get; }

        IObservable<double> Progress { get; }

        IObservable<WorkerState> ReaderState { get; }

        IObservable<WorkerState> WriterState { get; }

        IObservable<ResultDto> Result { get; }
    }
}