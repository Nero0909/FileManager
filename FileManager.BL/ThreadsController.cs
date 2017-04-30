using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Unity;
using FileManager.BL.Interfaces.Workers;
using FileManager.BL.Workers;

namespace FileManager.BL
{
    public sealed class ThreadsController : IThreadsController
    {
        private readonly IFactory<IFileReader> _readerFactory;
        private readonly IFactory<IFileWriter> _writerFactory;
        private readonly IFactory<IBytesBuffer> _bufferFactory;

        private readonly BehaviorSubject<double> _sizeObs;
        private readonly BehaviorSubject<double> _progressObs;

        private readonly BehaviorSubject<WorkerState> _readerState;
        private readonly BehaviorSubject<WorkerState> _writerState;

        private readonly Subject<ResultDto> _result;

        private IFileReader _reader;
        private IFileWriter _writer;
        private IBytesBuffer _buffer;

        private CancellationTokenSource _cancellationTokenSource;

        public ThreadsController(IFactory<IFileReader> readerFactory,
            IFactory<IFileWriter> writerFactory,
            IFactory<IBytesBuffer> bufferFactory)
        {
            _readerFactory = readerFactory;
            _writerFactory = writerFactory;
            _bufferFactory = bufferFactory;

            _result = new Subject<ResultDto>();
            _sizeObs = new BehaviorSubject<double>(0.0);
            _progressObs = new BehaviorSubject<double>(0.0);
            _readerState = new BehaviorSubject<WorkerState>(WorkerState.Unstarted);
            _writerState = new BehaviorSubject<WorkerState>(WorkerState.Unstarted);
        }

        public IObservable<double> CurrentBufferSize => _sizeObs;

        public IObservable<double> Progress => _progressObs;

        public IObservable<WorkerState> ReaderState => _readerState;

        public IObservable<WorkerState> WriterState => _writerState;

        public IObservable<ResultDto> Result => _result;

        public void StartReadAndWrite(string srcPath, string destPath, int bufferSize)
        {
            double totalLength = new FileInfo(srcPath).Length;
            _cancellationTokenSource = new CancellationTokenSource();

            if (_buffer == null)
            {
                _buffer = _bufferFactory.ConstructWith(bufferSize).Create();
            }
            else
            {
                _buffer.Reset(bufferSize);
            }

            _reader = _readerFactory.ConstructWith(_buffer).And(_cancellationTokenSource).Create();
            _writer = _writerFactory.ConstructWith(_buffer).And(_cancellationTokenSource).Create();

            var readerObs = _reader.GetReadBytesStream(srcPath);
            var writerObs = _writer.GetWriteBytesStream(destPath);

            SubscribeOnProgressChanges(writerObs, totalLength);
            SubscribeOnBufferSizeChanges(readerObs, writerObs, bufferSize);
            SubscribeOnThreadsState();
            SubscribeOnError();
            SubscribeOnComplete();

            readerObs.Connect();
            writerObs.Connect();
        }

        private void SubscribeOnProgressChanges(IObservable<int> writer, double totalLength)
        {
            writer
                .Scan(0, (acc, currentVal) => acc + currentVal)
                .Select(x => x / totalLength)
                .Concat(Observable.Return(0.0))
                .Subscribe(prgs => _progressObs.OnNext(prgs));
        }

        private void SubscribeOnBufferSizeChanges(IObservable<int> reader, IObservable<int> writer, int maxBufferSize)
        {
            reader
                .Merge(writer.Select(numWrite => -numWrite))
                .Scan(0, (acc, current) => acc + current)
                .Select(x => x / (double) maxBufferSize)
                .Concat(Observable.Return(0.0))
                .Subscribe(size => _sizeObs.OnNext(size));
        }

        private void SubscribeOnThreadsState()
        {
            _writer.CurrentState.Subscribe(state => _writerState.OnNext(state));
            _reader.CurrentState.Subscribe(state => _readerState.OnNext(state));
        }

        private void SubscribeOnError()
        {
            _reader.Result.Amb(_writer.Result).Where(state => state.Result == Workers.Result.Error).Subscribe(r =>
            {
                Cancel();
                _result.OnNext(r);
            });
        }

        private void SubscribeOnComplete()
        {
            _reader.Result.CombineLatest(_writer.Result, (r, w) => new {ReaderResult = r, WriterResult = w}).Subscribe(r =>
            {
                if (r.ReaderResult.Result != Workers.Result.Error && r.WriterResult.Result != Workers.Result.Error)
                {
                    _result.OnNext(r.ReaderResult);
                }
                _buffer.Clear();
            });
        }
        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void PauseWriting()
        {
            _writer?.Pause();
        }

        public void ResumeWriting()
        {
            _writer.Resume();
        }

        public void PauseReading()
        {
            _reader?.Pause();
        }

        public void ResumeReading()
        {
            _reader?.Resume();
        }
    }
}
