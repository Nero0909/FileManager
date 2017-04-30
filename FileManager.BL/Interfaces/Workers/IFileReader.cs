using System.Reactive.Subjects;

namespace FileManager.BL.Interfaces.Workers
{
    public interface IFileReader : ISuspendableWorker
    {
        IConnectableObservable<int> GetReadBytesStream(string path);
    }
}