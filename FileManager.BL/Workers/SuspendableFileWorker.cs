using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using SystemInterface.IO;
using FileManager.BL.Interfaces;
using Nito.AsyncEx;

namespace FileManager.BL.Workers
{
    internal abstract class SuspendableFileWorker : ISuspendableWorker
    {
        protected IBytesBuffer Buffer;
        protected CancellationTokenSource CancellationTokenSource;
        protected PauseTokenSource PauseTokenSource;
        private readonly BehaviorSubject<WorkerState> _currentStateObs;
        private readonly AsyncSubject<ResultDto> _result;
        protected IFile FileWrapper;

        protected SuspendableFileWorker(IBytesBuffer buffer, IFile fileWrapper, CancellationTokenSource cancellationTokenSource)
        {
            Buffer = buffer;
            CancellationTokenSource = cancellationTokenSource;
            FileWrapper = fileWrapper;

            PauseTokenSource = new PauseTokenSource();
            _currentStateObs = new BehaviorSubject<WorkerState>(WorkerState.Unstarted);
            _result = new AsyncSubject<ResultDto>();
        }

        public void Pause()
        {
            if (PauseTokenSource.IsPaused) return;

            PauseTokenSource.IsPaused = true;
            _currentStateObs.OnNext(WorkerState.Suspended);
        }

        public void Resume()
        {
            if (!PauseTokenSource.IsPaused) return;

            PauseTokenSource.IsPaused = false;
            _currentStateObs.OnNext(WorkerState.Running);
        }

        public void Cancel()
        {
            CancellationTokenSource.Cancel();
            _currentStateObs.OnNext(WorkerState.Stopped);
        }

        protected IConnectableObservable<int> DoWork(string filePath)
        {
            var subscription = DoWorkInternal(filePath).Publish();

            subscription
                .Take(1)
                .Subscribe(
                    _ => _currentStateObs.OnNext(WorkerState.Running));

            subscription
                .IgnoreElements()
                .Subscribe(
                    _ => { },
                    ex =>
                    {
                        _currentStateObs.OnNext(WorkerState.Stopped);
                        _result.OnNext(new ResultDto(Workers.Result.Error, ex));
                        _result.OnCompleted();
                    },
                    () =>
                    {
                        _currentStateObs.OnNext(WorkerState.Stopped);
                       var result = CancellationTokenSource.Token.IsCancellationRequested
                            ? new ResultDto(Workers.Result.Canceled)
                            : new ResultDto(Workers.Result.Successfully);

                        _result.OnNext(result);
                        _result.OnCompleted();
                    });

            return subscription;
        }

        protected abstract IObservable<int> DoWorkInternal(string filePath); 

        public IObservable<WorkerState> CurrentState => _currentStateObs.DistinctUntilChanged();

        public IObservable<ResultDto> Result => _result;
    }
}