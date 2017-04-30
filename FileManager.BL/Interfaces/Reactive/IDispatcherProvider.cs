using System;
using System.Reactive.Concurrency;

namespace FileManager.BL.Interfaces.Reactive
{
    public interface IDispatcherProvider
    {
        IScheduler Scheduler { get; }

        void Invoke(Action action);
    }
}