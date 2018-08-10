using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace ContestMeter.Web.Site.Queue
{
    public class CheckSolutionQueue : IDisposable
    {
        private ConcurrentQueue<Task> Tasks;

        private int runningTasksCount;

        private int wokringInterval;

        private CancellationTokenSource cancelToken;

        public CheckSolutionQueue(int workingIntervalMillisecond, int maxRunningTasks)
        {
            runningTasksCount = 0;

            Tasks = new ConcurrentQueue<Task>();
            wokringInterval= workingIntervalMillisecond;

            cancelToken = new CancellationTokenSource();
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                while (true)
                {

                    if (Tasks.IsEmpty)
                        Thread.Sleep(wokringInterval);
                    else
                    {
                        if (runningTasksCount == maxRunningTasks)
                        {
                            Thread.Sleep(100);
                            continue;
                        }
                        Task task;
                        Tasks.TryDequeue(out task);
                        runningTasksCount++;
                        task.Start();
                        task.ContinueWith((x) => runningTasksCount--);
                    }
                    if (cancelToken.IsCancellationRequested)
                        break;
                }
            }, cancelToken.Token);
        }

        public void AddTask(Task task)
        {
            Tasks.Enqueue(task);
        }

        public void Dispose()
        {
            cancelToken.Cancel();
        }
    }
}