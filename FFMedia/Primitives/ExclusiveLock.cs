using System.Diagnostics;

namespace FFMedia.Primitives;

/// <summary>
/// Implements a thread synchronization primitive that
/// does not require thread affinity and provides constructs for using
/// statements.
/// See: http://kflu.github.io/2017/04/04/2017-04-04-csharp-synchronization/
/// and: https://blog.cdemi.io/async-waiting-inside-c-sharp-locks/
/// </summary>
public sealed class ExclusiveLock : IDisposable
{
    const int InitialFreeSlots = 1;
    const int AvailableSlots = 1;

    private bool m_IsDisposed;
    private readonly object SyncRoot = new();
    private readonly SemaphoreSlim Semaphore = new(InitialFreeSlots, AvailableSlots);
    private IDisposable? CurrentLock;

    private bool IsAvailable
    {
        get
        {
            lock (SyncRoot)
                return !m_IsDisposed && CurrentLock is null && Semaphore.CurrentCount > 0;
        }

    }

    private bool IsDisposed
    {
        get
        {
            lock (SyncRoot)
                return m_IsDisposed;
        }
    }

    /// <summary>
    /// Tries to acquire a lock immediately and without blocking.
    /// If the locking operation fails, returns a null value.
    /// If the locking operation succeeds, returns a <see cref="IDisposable"/>
    /// which unlocks the operation upon calling <see cref="IDisposable.Dispose"/>.
    /// </summary>
    /// <returns>The disposable locking object.</returns>
    public IDisposable? TryLock()
    {
        lock (SyncRoot)
        {
            if (!IsAvailable || Semaphore.Wait(0) == false)
                return null;

            var locker = new LockReleaser(this);
            return locker;
        }
    }

    /// <summary>
    /// Tries to acquire a lock by indefinitely blocking.
    /// If the locking operation fails, returns a null value.
    /// If the locking operation succeeds, returns a <see cref="IDisposable"/>
    /// which unlocks the operation upon calling <see cref="IDisposable.Dispose"/>.
    /// </summary>
    /// <remarks>
    /// The only instance in which locking fails is when <see cref="IDisposable.Dispose"/>
    /// is called on this <see cref="ExclusiveLock"/>.
    /// </remarks>
    /// <returns>The disposable locking object.</returns>
    public IDisposable? Lock()
    {
        var locker = TryLock();
        while (!IsDisposed && locker is null)
        {
            Thread.Sleep(1);
            locker = TryLock();
        }

        return locker;
    }

    /// <summary>
    /// Tries to acquire a lock by blocking at least the specified number of milliseconds.
    /// If the locking operation fails, returns a null value.
    /// If the locking operation succeeds, returns a <see cref="IDisposable"/>
    /// which unlocks the operation upon calling <see cref="IDisposable.Dispose"/>.
    /// </summary>
    /// <remarks>
    /// The locking fails when <see cref="IDisposable.Dispose"/>
    /// is called on this <see cref="ExclusiveLock"/> or the timeout has elapsed.
    /// </remarks>
    /// <returns>The disposable locking object.</returns>
    public IDisposable? Lock(int millisecondsTimeout)
    {
        var startTime = Stopwatch.GetTimestamp();
        if (millisecondsTimeout < 0) return Lock();

        var locker = TryLock();
        while (!IsDisposed && locker is null &&
            Stopwatch.GetElapsedTime(startTime).TotalMilliseconds < millisecondsTimeout)
        {
            Thread.Sleep(1);
            locker = TryLock();
        }

        return locker;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        lock (SyncRoot)
        {
            if (m_IsDisposed)
                return;

            m_IsDisposed = true;

            if (CurrentLock is not null)
            {
                CurrentLock.Dispose();
                CurrentLock = null;
            }

            Semaphore.Dispose();
        }
    }

    /// <summary>
    /// Implements the locks released.
    /// Dispose causes the necessary side-effects to the parent.
    /// </summary>
    private class LockReleaser : IDisposable
    {
        private ExclusiveLock? Parent;
        private long m_IsDisposed;

        public LockReleaser(ExclusiveLock parent)
        {
            Parent = parent;
            Parent.CurrentLock = this;
        }

        public void Dispose()
        {
            if (Interlocked.Increment(ref m_IsDisposed) > 1)
                return;

            if (Parent is not null && !Parent.IsDisposed)
            {
                Parent.Semaphore.Release();
                Parent.CurrentLock = null;
            }

            Parent = null;
        }
    }
}
