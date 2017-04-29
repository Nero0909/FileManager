using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FileManager.BL.Reactive
{
    public static class ObservableProgress
    {
        public static IObservable<T> CreateAsync<T>(Func<IProgress<T>, Task> action)
        {
            return Observable.Create<T>(async obs =>
            {
                try
                {
                    await action(new DelegateProgress<T>(obs.OnNext));
                    obs.OnCompleted();
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                }
            });
        }

        private sealed class DelegateProgress<T> : IProgress<T>
        {
            private readonly Action<T> _report;

            public DelegateProgress(Action<T> report)
            {
                if (report == null) throw new ArgumentNullException();
                _report = report;
            }

            public void Report(T value)
            {
                _report(value);
            }
        }
    }
}
