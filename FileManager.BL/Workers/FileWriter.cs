using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using SystemInterface.IO;
using FileManager.BL.Interfaces;
using FileManager.BL.Interfaces.Workers;
using FileManager.BL.Reactive;

namespace FileManager.BL.Workers
{
    using IStream = SystemInterface.IO.IStream;

    internal sealed class FileWriter : SuspendableFileWorker, IFileWriter
    {
        public FileWriter(
            IBytesBuffer buffer, 
            IFile fileWrapper,
            CancellationTokenSource cancellationTokenSource) : base(buffer, fileWrapper, cancellationTokenSource)
        {
        }

        public IConnectableObservable<int> GetWriteBytesStream(string path)
        {
            return DoWork(path);
        }

        protected override IObservable<int> DoWorkInternal(string path)
        {
            return ObservableProgress.CreateAsync<int>(
                progressReporter => WriteFile(path, progressReporter));
        }

        private async Task WriteFile(string url, IProgress<int> progressReporter)
        {
            try
            {
                using (var fs = FileWrapper.OpenWrite(url))
                {
                    while (await Buffer.OutputAvailableAsync(CancellationTokenSource.Token))
                    {
                        await PauseTokenSource.Token.WaitWhilePausedAsync(CancellationTokenSource.Token);
                        var segment = await Buffer.GetFilledSegmentAsync(CancellationTokenSource.Token);

                        await fs.WriteAsync(segment.Array, segment.Offset, segment.Count, CancellationTokenSource.Token);

                        progressReporter.Report(segment.Count);

                        await Buffer.FreeSegmentAsync(segment, CancellationTokenSource.Token);
                    }
                }
            }
            catch (Exception)
            {
                DeleteIfExists(url);
                throw;
            }
        }

        private void DeleteIfExists(string url)
        {
            if (FileWrapper.Exists(url))
            {
                FileWrapper.Delete(url);
            }
        }
    }
}