using System.Collections;

namespace FFMedia.Engine;

public partial class FrameStore<TMedia>
{
    internal sealed class FrameGraph(FrameStore<TMedia> parent, int initialCapacity) :
        IList<TMedia>,
        IFrameGraph<TMedia>
    {
        private readonly SortedList<TimeExtent, TMedia> Frames = new(initialCapacity);
        private readonly FrameStore<TMedia> Parent = parent;

        /// <inheritdoc />
        public TMedia this[int index]
        {
            get => Frames.GetValueAtIndex(index);
            set => throw new NotSupportedException($"Setting an item at an index of this graph defeats its purpose.");
        }

        /// <inheritdoc />
        public IMediaComponent<TMedia> Component => Parent.Component;

        /// <inheritdoc />
        public int Count => Frames.Count;

        /// <inheritdoc />
        public bool IsFull => Frames.Count >= Parent.Capacity;

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public bool IsEmpty => Frames.Count <= 0;

        /// <inheritdoc />
        public int MinIndex => IsEmpty ? -1 : 0;

        /// <inheritdoc />
        public int MaxIndex => Frames.Count - 1;

        /// <inheritdoc />
        public TimeExtent MaxStartTime => IsEmpty
            ? TimeExtent.NaN
            : Frames[MaxIndex].StartTime;

        /// <inheritdoc />
        public TimeExtent RangeStartTime => IsEmpty
            ? TimeExtent.NaN
            : Frames[MinIndex].StartTime;

        /// <inheritdoc />
        public TimeExtent RangeEndTime => IsEmpty
            ? TimeExtent.NaN
            : Frames[MaxIndex].StartTime + Frames[MaxIndex].Duration;

        /// <inheritdoc />
        public TimeExtent TotalDuration => IsEmpty
            ? TimeExtent.Zero
            : RangeEndTime - RangeStartTime;

        /// <inheritdoc />
        public int GroupIndex { get; set; }

        /// <inheritdoc />
        public void Add(TMedia item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (item.StartTime.IsNaN)
                throw new ArgumentException($"{nameof(item)} '{nameof(item.StartTime)}' has to be finite.");

            if (item.Duration.IsNaN || item.Duration < TimeExtent.Zero)
                throw new ArgumentException($"{nameof(item)} '{nameof(item.Duration)}' has to be finite and non-negative.");

            if (Frames.ContainsValue(item))
                throw new ArgumentException($"{nameof(item)} is already contained in the set.");

            if (Frames.ContainsKey(item.StartTime))
                throw new ArgumentException($"An {nameof(item)} with the same {nameof(item.StartTime)} is already contained in the set.");

            // Automatically make room for the new frame.
            if (IsFull)
            {
                var relativePosition = FindRelativePosition(item);
                if (relativePosition < 0.5)
                    RemoveLast();
                else
                    RemoveFirst();
            }

            Frames.Add(item.StartTime, item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            while (Frames.Count > 0)
                RemoveAt(0);
        }

        /// <inheritdoc />
        public bool Contains(TMedia item) => Frames.ContainsValue(item);

        /// <inheritdoc />
        public bool Contains(TimeExtent time) => !time.IsNaN && !IsEmpty &&
            time >= RangeStartTime && time <= RangeEndTime;

        /// <inheritdoc />
        public void CopyTo(TMedia[] array, int arrayIndex) => Frames.Values.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<TMedia> GetEnumerator() => Frames.Values.GetEnumerator();

        /// <inheritdoc />
        public int IndexOf(TMedia item) => Frames.IndexOfValue(item);

        /// <inheritdoc />
        public void Insert(int index, TMedia item) =>
            throw new NotSupportedException($"Inserting an item at an index of this graph defeats its purpose.");

        /// <inheritdoc />
        public bool Remove(TMedia item)
        {
            var index = Frames.IndexOfValue(item);
            if (index < 0)
                return false;

            RemoveAt(index);
            return true;
        }

        public bool RemoveFirst()
        {
            if (IsEmpty)
                return false;

            RemoveAt(MinIndex);
            return true;
        }

        public bool RemoveLast()
        {
            if (IsEmpty)
                return false;

            RemoveAt(MaxIndex);
            return true;
        }

        /// <inheritdoc />
        public void RemoveAt(int index)
        {
            var frame = Frames[index];
            Component.FramePool.ReturnFrameLease(frame);
            Frames.RemoveAt(index);
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => Frames.Values.GetEnumerator();

        /// <inheritdoc />
        public TMedia? FindFrame(TimeExtent startTime)
        {
            var keyIndex = Frames.Keys.IndexOfClosest(startTime, out _);
            if (keyIndex < 0)
                return null;

            return Frames.GetValueAtIndex(keyIndex);
        }

        /// <inheritdoc />
        public int FindFrameIndex(TimeExtent startTime) =>
            Frames.Keys.IndexOfClosest(startTime, out _);

        /// <inheritdoc />
        public double FindRelativePosition(TimeExtent time)
        {
            if (IsEmpty)
                return 0;

            if (time.IsNaN)
                throw new ArgumentException($"{nameof(time)} has to be finite.");

            var timeOffset = (time - RangeStartTime).Milliseconds;
            return timeOffset / TotalDuration.Milliseconds;
        }

        /// <inheritdoc />
        public double FindRelativePosition(TMedia item)
        {
            ArgumentNullException.ThrowIfNull(item);

            if (item.StartTime.IsNaN)
                throw new ArgumentException($"{nameof(item)} '{nameof(item.StartTime)}' has to be finite.");

            if (item.Duration.IsNaN || item.Duration < TimeExtent.Zero)
                throw new ArgumentException($"{nameof(item)} '{nameof(item.Duration)}' has to be finite and non-negative.");

            return FindRelativePosition(item.StartTime + (item.Duration / 2d));
        }

        /// <inheritdoc />
        public void Dispose() => Parent.Unlock();
    }
}
