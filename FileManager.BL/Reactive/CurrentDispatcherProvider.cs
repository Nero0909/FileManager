using System;
using System.Reactive.Concurrency;
using System.Windows;
using System.Windows.Threading;
using FileManager.BL.Interfaces.Reactive;

namespace FileManager.BL.Reactive
{
    internal sealed class CurrentDispatcherProvider : IDispatcherProvider
    {
        public IScheduler Scheduler
        {
            get
            {
                var application = Application.Current;
                if (application != null)
                {
                    return new DispatcherScheduler(application.Dispatcher);
                }
                return new DispatcherScheduler(Dispatcher.CurrentDispatcher);
            }
        }

        public void Invoke(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var appliication = Application.Current;
            appliication?.Dispatcher.Invoke(action);
        }
    }
}
