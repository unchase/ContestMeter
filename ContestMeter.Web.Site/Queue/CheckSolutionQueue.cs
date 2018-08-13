using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ContestMeter.Web.Site.Queue
{
    public class CheckSolutionQueue : IDisposable
    {
        private readonly ConcurrentQueue<Task> _tasks;

        private int _runningTasksCount;

        private int _wokringInterval;

        private readonly CancellationTokenSource _cancelToken;

        public CheckSolutionQueue(int workingIntervalMillisecond, int maxRunningTasks)
        {
            _runningTasksCount = 0;

            _tasks = new ConcurrentQueue<Task>();
            _wokringInterval= workingIntervalMillisecond;

            _cancelToken = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    if (_tasks.IsEmpty)
                        Thread.Sleep(_wokringInterval);
                    else
                    {
                        if (_runningTasksCount == maxRunningTasks)
                        {
                            Thread.Sleep(100);
                            continue;
                        }

                        _tasks.TryDequeue(out var task);
                        _runningTasksCount++;
                        task.Start();
                        task.ContinueWith(x => _runningTasksCount--);
                    }
                    if (_cancelToken.IsCancellationRequested)
                        break;
                }
            }, _cancelToken.Token);
        }

        public void AddTask(Task task)
        {
            _tasks.Enqueue(task);
        }

        public void Dispose()
        {
            _cancelToken.Cancel();
        }
    }
}