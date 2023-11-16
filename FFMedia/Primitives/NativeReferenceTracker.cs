#pragma warning disable CA1822
using System.Diagnostics;

namespace FFMedia.Primitives;

/// <summary>
/// Tracks <see cref="INativeTrackedReference"/> objects
/// to ensure they are properly disposed of and provides
/// means for diagnosing unmanaged memory leaks.
/// </summary>
public sealed class NativeReferenceTracker
{
    private static readonly object SyncLock = new();
    private static readonly SortedDictionary<ulong, (INativeTrackedReference obj, string? source)> Graph = [];
    private static ulong LastObjectId;
    private static ulong m_Count;
    private static readonly NativeReferenceTracker m_Instance = new();

    /// <summary>
    /// Prevents instantiation of this class as it is a singleton.
    /// </summary>
    private NativeReferenceTracker()
    {
        // placeholder
    }

    /// <summary>
    /// Gest the <see cref="NativeReferenceTracker"/> instance of this singleton.
    /// </summary>
    public static NativeReferenceTracker Instance => m_Instance;

    /// <summary>
    /// Adds a reference to the tracker and returns the object id assigned to it.
    /// </summary>
    /// <param name="item">The tracked reference to add.</param>
    /// <param name="source">The name of the module or source code location where this item was allocated.</param>
    /// <returns>The acquired object id.</returns>
    public ulong Add(INativeTrackedReference item, string? source)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (SyncLock)
        {
            var objectId = LastObjectId;
            Graph.Add(LastObjectId, (item, source));
            LastObjectId++;
            m_Count++;
            return objectId;
        }
    }

    /// <summary>
    /// Removes a <see cref="INativeTrackedReference"/> from the tracker.
    /// Only call this method after native memory has been freed.
    /// </summary>
    /// <param name="item">The tracked reference to remove.</param>
    public void Remove(INativeTrackedReference item)
    {
        ArgumentNullException.ThrowIfNull(item);

        lock (SyncLock)
        {
            if (!Graph.ContainsKey(item.ObjectId))
                return;

            Graph.Remove(item.ObjectId);
            m_Count--;
        }
    }

    /// <summary>
    /// Gets the number of items that are currently alive in the tracker.
    /// </summary>
    public ulong Count
    {
        get
        {
            lock (SyncLock)
                return m_Count;
        }
    }

    /// <summary>
    /// Asserts that the count of alive objects is zero.
    /// </summary>
    public void VerifyZero()
    {
        // TODO: Remove this
        if (Count != 0)
            Debug.Assert(Count == 0);
    }
}
#pragma warning restore CA1822