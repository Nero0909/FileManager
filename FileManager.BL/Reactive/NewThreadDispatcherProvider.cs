using System;
using System.Reactive.Concurrency;
using FileManager.BL.Interfaces.Reactive;

namespace FileManager.BL.Reactive
{
    internal sealed class NewThreadDispatcherProvider : IDispatcherProvider
    {
        public IScheduler Scheduler
        {
            get { return NewThreadScheduler.Default; }
        }

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            Scheduler.Schedule(action);
        }
    }
}
