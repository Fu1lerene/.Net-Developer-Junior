using System.Collections.Concurrent;
using Homework_3.Configurations;
using Microsoft.Extensions.Options;

namespace Homework_3.Domain.CustomTaskSchedulers;

public class CustomTaskScheduler : TaskScheduler, IDisposable
{
    private IOptionsMonitor<AppSettings> _appSettings;
    private Thread[] _threads;
    private readonly ConcurrentQueue<Task> _tasks;
    private readonly ManualResetEventSlim _sync;
    private SemaphoreSlim _semaphore;
    private int _oldMaxDegree;
    private bool _isDisposed;

    public CustomTaskScheduler(IOptionsMonitor<AppSettings> appSettings, CancellationToken cancellationToken)
    {
        _appSettings = appSettings;
        _oldMaxDegree = appSettings.CurrentValue.MaxDegreeOfParallelism;
        _semaphore = new(appSettings.CurrentValue.MaxDegreeOfParallelism);
        _sync = new(false);
        _tasks = new();
        _threads = new Thread[appSettings.CurrentValue.MaxDegreeOfParallelism];
        
        for (int i = 0; i < appSettings.CurrentValue.MaxDegreeOfParallelism; i++)
        {
            _threads[i] = new Thread(Run)
            {
                IsBackground = true
            };
            _threads[i].Start();
        }
        _appSettings.OnChange((a, b) =>
        {
            SetMaxDegreeOfParallelism();
        });
    }

    public void SetMaxDegreeOfParallelism()
    {
        if (_appSettings.CurrentValue.MaxDegreeOfParallelism < 1)
            throw new ArgumentException("Max degree of parallelism should be more than 0");

        if (_appSettings.CurrentValue.MaxDegreeOfParallelism > _threads.Length)
        {
            Array.Resize(ref _threads, _appSettings.CurrentValue.MaxDegreeOfParallelism);
            for (int i = _oldMaxDegree; i < _appSettings.CurrentValue.MaxDegreeOfParallelism; i++)
            {
                _threads[i] = new Thread(Run)
                {
                    IsBackground = true
                };
                _threads[i].Start();
            }
        }

        _oldMaxDegree = _appSettings.CurrentValue.MaxDegreeOfParallelism;
        
    }
    
    private void Run()
    {
        while (!_isDisposed)
        {
            _sync.Wait();
            if (_tasks.TryDequeue(out var task))
            {
                if (!TryExecuteTask(task))
                {
                    Console.WriteLine("Не удалось");
                }
            }
            else
            {
                _sync.Reset();
            }
        }
    }
    
    protected override void QueueTask(Task task)
    {
        _tasks.Enqueue(task);
        _sync.Set();
    }
    
    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return new List<Task>();
    }
    
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return false;
    }

    public void Dispose()
    {
        _isDisposed = true;
        _sync.Set();
    }
}