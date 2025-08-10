namespace npm.api.Core
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class ThreadWorker<T>
    {
        private ConcurrentQueue<T> Datas = null;
        private ManualResetEvent[] Lockers = null;
        private List<Thread> Threads = new List<Thread>(10);

        public int ThreadCount { get; set; } = 9;

        public Task Start(IEnumerable<T> datas, Action<T> work)
        {
            Datas = new ConcurrentQueue<T>(datas);
            Lockers = new ManualResetEvent[ThreadCount];

            Threads.Clear();

            for (int i = 0; i < ThreadCount; i++)
            {
                var locker = new ManualResetEvent(false);
                var t = new Thread(() =>
                {
                    while (!Datas.IsEmpty)
                    {
                        while (Datas.TryDequeue(out T data))
                        {
                            work.Invoke(data);
                        }

                        Thread.Sleep(75);
                    }

                    locker.Set();
                });
                t.Name = $"ThreadWorker-{i + 1}";
                Threads.Add(t);
                Lockers[i] = locker;

                t.Start();
            }

            return Task.Run(() => WaitHandle.WaitAll(Lockers));
        }

        public void Stop()
        {
            Datas = new ConcurrentQueue<T>();
        }
    }
}
