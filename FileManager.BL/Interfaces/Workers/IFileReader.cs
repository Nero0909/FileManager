using System.Reactive.Subjects;

namespace FileManager.BL.Interfaces.Workers
{
    public interface IFileReader : ISuspendable
    {
        IConnectableObservable<int> GetReadBytesStream(string path);
    }
}