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
    internal sealed class FileReader : SuspendableFileWorker, IFileReader
    {
        public FileReader(IBytesBuffer buffer, IFile fileWrapper, CancellationTokenSource cancellationTokenSource)
            : base(buffer, fileWrapper, cancellationTokenSource)
        {
        }

        public IConnectableObservable<int> GetReadBytesStream(string path)
        {
            return DoWork(path);
        }

        protected override IObservable<int> DoWorkInternal(string filePath)
        {
            return ObservableProgress.CreateAsync<int>(
                progressReporter => ReadFile(filePath, progressReporter));
        }

        private async Task ReadFile(string url, IProgress<int> progressReporter)
        {
            try
            {
                using (var fs = FileWrapper.OpenRead(url))
                {
                    var bytesRead = 0;
                    do
                    {
                        await PauseTokenSource.Token.WaitWhilePausedAsync(CancellationTokenSource.Token);

                        var segment = await Buffer.GetEmptySegmentAsync(CancellationTokenSource.Token);

                        bytesRead = await fs.ReadAsync(segment.Array, segment.Offset, segment.Count,
                            CancellationTokenSource.Token);
                        progressReporter.Report(bytesRead);

                        await Buffer.FillSegmentAsync(segment, bytesRead, CancellationTokenSource.Token);

                    } while (bytesRead > 0);
                }

            }
            finally
            {
                Buffer.CompleteAdding();
            }
        }
    }
}