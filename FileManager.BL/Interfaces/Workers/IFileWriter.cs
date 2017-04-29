using System.Reactive.Subjects;

namespace FileManager.BL.Interfaces.Workers
{
    public interface IFileWriter : ISuspendable
    {
        IConnectableObservable<int> GetWriteBytesStream(string path);
    }
}