using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly BehaviorSubject<int> _sizeObs;
        private readonly BehaviorSubject<double> _progressObs;

        private readonly SerialDisposable _sizeAnchor;
        private readonly SerialDisposable _progressAnchor;
        private readonly SerialDisposable _readerStateAnchor;
        private readonly SerialDisposable _writerStateAnchor;

        private readonly BehaviorSubject<WorkerState> _readerState;
        private readonly BehaviorSubject<WorkerState> _writerState;

        private IFileReader _reader;
        private IFileWriter _writer;

        private CancellationTokenSource _cancellationTokenSource;

        public ThreadsController(
            IFactory<IFileReader> readerFactory,
            IFactory<IFileWriter> writerFactory,
            IFactory<IBytesBuffer> bufferFactory)
        {
            _readerFactory = readerFactory;
            _writerFactory = writerFactory;
            _bufferFactory = bufferFactory;

            _sizeObs = new BehaviorSubject<int>(0);
            _progressObs = new BehaviorSubject<double>(0.0);
            _readerState = new BehaviorSubject<WorkerState>(WorkerState.Unstarted);
            _writerState = new BehaviorSubject<WorkerState>(WorkerState.Unstarted);

            _sizeAnchor = new SerialDisposable();
            _progressAnchor = new SerialDisposable();
            _writerStateAnchor = new SerialDisposable();
            _readerStateAnchor = new SerialDisposable();
        }

        public IObservable<int> CurrentBufferSize => _sizeObs;

        public IObservable<double> Progress => _progressObs;

        public IObservable<WorkerState> ReaderState => _readerState;

        public IObservable<WorkerState> WriterState => _writerState;

        public void StartReadAndWrite(string srcPath, string destPath, int bufferSize)
        {
            var totalLength = new FileInfo(srcPath).Length;
            _cancellationTokenSource = new CancellationTokenSource();
            var buffer = _bufferFactory.ConstructWith(bufferSize).Create();

            _reader = _readerFactory.ConstructWith(buffer).And(_cancellationTokenSource).Create();
            _writer = _writerFactory.ConstructWith(buffer).And(_cancellationTokenSource).Create();

            var readerObs = _reader.GetReadBytesStream(srcPath);
            var writerObs = _writer.GetWriteBytesStream(destPath);

            _progressAnchor.Disposable = readerObs
                .Scan(0, (acc, currentVal) => acc + currentVal)
                .Sample(TimeSpan.FromMilliseconds(50))
                .Select(x => x / totalLength)
                .Subscribe(prgs => _progressObs.OnNext(prgs));

            _sizeAnchor.Disposable = readerObs
                .Merge(writerObs.Select(numWrite => -numWrite))
                .Scan(0, (acc, current) => acc + current)
                .Subscribe(size => _sizeObs.OnNext(size));

            _writerStateAnchor.Disposable = _writer.CurrentState.Subscribe(state => _writerState.OnNext(state));
            _readerStateAnchor.Disposable = _reader.CurrentState.Subscribe(state => _readerState.OnNext(state));

            readerObs.Connect();
            writerObs.Connect();
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
