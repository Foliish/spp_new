using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingFramework.Runner
{
    public class CustomThreadPool : IDisposable
    {
        private readonly Queue<Action> _tasks = new Queue<Action>();
        private readonly List<Thread> _workers = new List<Thread>();

        private readonly int _minThreads;
        private readonly int _maxThreads;
        private readonly int _idleTimeoutMs;

        private bool _isRunning = true;
        private int _activeThreads = 0; 
        private readonly object _lock = new object();

        public CustomThreadPool(int minThreads, int maxThreads, int idleTimeoutMs = 5000)
        {
            _minThreads = minThreads;
            _maxThreads = maxThreads;
            _idleTimeoutMs = idleTimeoutMs;

            lock (_lock)
            {
                for (int i = 0; i < _minThreads; i++)
                {
                    CreateWorker();
                }
                LogState("Pool Initialized");
            }
        }
        public event EventHandler<string> StateChanged;
        public event EventHandler<string> ErrorOccurred;

        private void LogState(string reason)
        {
            string message = $"[{DateTime.Now:HH:mm:ss.fff}] {reason,-20} | Workers: {_workers.Count} | Active: {_activeThreads} | Tasks in Queue: {_tasks.Count}";
            StateChanged?.Invoke(this, message);
        }

        public void Enqueue(Action task)
        {
            lock (_lock)
            {
                _tasks.Enqueue(task);

                if (_activeThreads >= _workers.Count && _workers.Count < _maxThreads)
                {
                    CreateWorker();
                }

                LogState("Task Enqueued");
                Monitor.Pulse(_lock);
            }
        }

        private void CreateWorker()
        {
            var thread = new Thread(WorkerLoop)
            {
                IsBackground = true,
                Name = $"PoolWorker-{Guid.NewGuid().ToString().Substring(0, 4)}"
            };

            _workers.Add(thread);
            thread.Start();
        }

        private void WorkerLoop()
        {
            while (true)
            {
                Action task = null;

                lock (_lock)
                {
                    while (_tasks.Count == 0 && _isRunning)
                    {
                        if (!Monitor.Wait(_lock, _idleTimeoutMs))
                        {
                            if (_workers.Count > _minThreads)
                            {
                                _workers.Remove(Thread.CurrentThread);
                                LogState("Worker Retired (Idle)");
                                return;
                            }
                        }
                    }

                    if (!_isRunning && _tasks.Count == 0)
                    {
                        _workers.Remove(Thread.CurrentThread);
                        return;
                    }

                    if (_tasks.Count > 0)
                    {
                        task = _tasks.Dequeue();
                        _activeThreads++;
                        LogState("Task Started");
                    }
                }

                if (task != null)
                {
                    try
                    {
                        task.Invoke();
                    }
                    catch (Exception ex)
                    {
                        ErrorOccurred?.Invoke(this, $"[ERROR] {ex.Message}");
                    }
                    finally
                    {
                        _activeThreads--;
                        LogState("Task Finished");
                        
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (_lock)
            {
                _isRunning = false;
                LogState("Shutting Down");
                Monitor.PulseAll(_lock);
            }
            List<Thread> workersCopy;
            lock (_lock) { workersCopy = new List<Thread>(_workers); }

            foreach (var worker in workersCopy)
            {
                if (worker.IsAlive && worker.ManagedThreadId != Thread.CurrentThread.ManagedThreadId) 
                    worker.Join();
            }

            StateChanged?.Invoke(this, "Pool Disposed.");
        }
    }
}
