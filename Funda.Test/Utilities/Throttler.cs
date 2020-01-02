using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Funda.Test.Utilities
{
    public class Throttler : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;
        private readonly ConcurrentQueue<int> _exitTimes;
        private readonly Timer _exitTimer;
        private readonly int _periodInMilliseconds;

        public Throttler(int limit, TimeSpan period)
        {
            _periodInMilliseconds = (int)period.TotalMilliseconds;
            _semaphore = new SemaphoreSlim(limit, limit);
            _exitTimes = new ConcurrentQueue<int>();
            _exitTimer = new Timer(state =>
            {
                // While there are exit times in the queue that are passed due still, exit the semaphore and dequeue the exit time.
                int exitTime;
                while (_exitTimes.TryPeek(out exitTime) && unchecked(exitTime - Environment.TickCount) <= 0)
                {
                    _semaphore.Release();
                    _exitTimes.TryDequeue(out _);
                }

                // Try to get the next exit time from the queue and compute the time until the next check should take place.
                // If the queue is empty, then no exit times will occur until at least one period has passed.
                _exitTimer.Change(_exitTimes.TryPeek(out exitTime) ? exitTime - Environment.TickCount : _periodInMilliseconds, -1);
            }, null, _periodInMilliseconds, Timeout.Infinite);
        }

        public void Throttle(CancellationToken cancellationToken)
        {
            _semaphore.Wait(cancellationToken);
            _exitTimes.Enqueue(unchecked(Environment.TickCount + _periodInMilliseconds));
        }

        public void Dispose()
        {
            _semaphore.Dispose();
            _exitTimer.Dispose();
        }
    }
}
