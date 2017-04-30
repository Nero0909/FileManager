using System.Reactive.Subjects;

namespace FileManager.BL.Interfaces.Workers
{
    public interface IFileWriter : ISuspendableWorker
    {
        IConnectableObservable<int> GetWriteBytesStream(string path);
    }
}