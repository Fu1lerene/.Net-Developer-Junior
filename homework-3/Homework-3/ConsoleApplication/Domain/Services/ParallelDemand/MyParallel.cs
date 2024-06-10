using System.Runtime.CompilerServices;

namespace Homework_3.Domain.Services.ParallelDemand;

public static class MyParallel
{
    public static Task DynamicParallelForEachAsync<TSource>(
        IAsyncEnumerable<TSource> source,
        DynamicParallelOptions options,
        Func<TSource, CancellationToken, ValueTask> body)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(body);

        SemaphoreSlim throttler = new(options.DegreeOfParallelism);
        options.DegreeOfParallelismChangedDelta += Options_ChangedDelta;
        void Options_ChangedDelta(object sender, int delta)
        {
            if (delta > 0)
                throttler.Release(delta);
            else
                for (int i = delta; i < 0; i++) throttler.WaitAsync();
        }

        async IAsyncEnumerable<TSource> GetThrottledSource(
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            IAsyncEnumerator<TSource> enumerator = source.GetAsyncEnumerator(
                cancellationToken);
            await using (enumerator.ConfigureAwait(false))
            {
                while (true)
                {
                    await throttler.WaitAsync().ConfigureAwait(false);
                    if (!await enumerator.MoveNextAsync().ConfigureAwait(false)) break;
                    yield return enumerator.Current;
                }
            }
        }

        return Parallel.ForEachAsync(GetThrottledSource(), options, async (item, ct) =>
        {
            try { await body(item, ct).ConfigureAwait(false); }
            finally { throttler.Release(); }
        }).ContinueWith(t =>
            {
                options.DegreeOfParallelismChangedDelta -= Options_ChangedDelta;
                return t;
            }, default, TaskContinuationOptions.DenyChildAttach |
                        TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default).Unwrap();
    }
}

public class DynamicParallelOptions : ParallelOptions
{
    private int _degreeOfParallelism;

    public event EventHandler<int> DegreeOfParallelismChangedDelta;

    public DynamicParallelOptions(int maxDegreeOfParallelism)
    {
        // The native Parallel.ForEachAsync will see the base.MaxDegreeOfParallelism.
        base.MaxDegreeOfParallelism = maxDegreeOfParallelism;
        _degreeOfParallelism = Environment.ProcessorCount;
    }

    public int DegreeOfParallelism
    {
        get { return _degreeOfParallelism; }
        set
        {
            if (value < 1) throw new ArgumentOutOfRangeException();
            if (value == _degreeOfParallelism) return;
            int delta = value - _degreeOfParallelism;
            DegreeOfParallelismChangedDelta?.Invoke(this, delta);
            _degreeOfParallelism = value;
        }
    }
}