namespace FileManager.BL.Interfaces.Unity
{
    public interface IFactory<out TOut>
    {
        TOut Create();

        TOut Create(string name);

        IFactory<TOut> ConstructWith<TWith>(TWith firstDependency);

        IFactory<TOut> And<T>(T dependency);
    }
}
