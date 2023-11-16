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

        public TMedia this[int index]
        {
            get => Frames.GetValueAtIndex(index);
            set => throw new NotSupportedException($"Setting an item at an index of this graph defeats its purpose.");
        }

        public IMediaComponent<TMedia> Component => Parent.Component;

        public int Count => Frames.Count;

        public bool IsFull => Frames.Count >= Parent.Capacity;

        public bool IsReadOnly => false;

        public bool IsEmpty => Frames.Count <= 0;

        public int MinIndex => IsEmpty ? -1 : 0;

        public int MaxIndex => Frames.Count - 1;

        public TimeExtent MinStartTime => IsEmpty
            ? TimeExtent.Zero
            : Frames[MinIndex].StartTime;

        public TimeExtent MaxStartTime => IsEmpty
            ? TimeExtent.Zero
            : Frames[MaxIndex].StartTime;

        public TimeExtent RangeStartTime => MinStartTime;

        public TimeExtent RangeEndTime => IsEmpty
            ? TimeExtent.Zero
            : Frames[MaxIndex].StartTime + Frames[MaxIndex].Duration;

        public TimeExtent TotalDuration => IsEmpty
            ? TimeExtent.Zero
            : RangeEndTime - RangeStartTime;

        public int GroupIndex { get; set; }

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

            Frames.Add(item.StartTime, item);
        }

        public void Clear() => Frames.Clear();

        public bool Contains(TMedia item) => Frames.ContainsValue(item);

        public bool Contains(TimeExtent time) => !time.IsNaN && !IsEmpty &&
            time >= RangeStartTime && time <= RangeEndTime;

        public void CopyTo(TMedia[] array, int arrayIndex) => Frames.Values.CopyTo(array, arrayIndex);

        public IEnumerator<TMedia> GetEnumerator() => Frames.Values.GetEnumerator();

        public int IndexOf(TMedia item) => Frames.IndexOfValue(item);

        public void Insert(int index, TMedia item) =>
            throw new NotSupportedException($"Inserting an item at an index of this graph defeats its purpose.");

        public bool Remove(TMedia item)
        {
            var index = Frames.IndexOfValue(item);
            if (index < 0)
                return false;

            Frames.RemoveAt(index);
            return true;
        }

        public bool Remove(TimeExtent startTime)
        {
            var index = FindFrameIndex(startTime);
            if (index < 0)
                return false;

            Frames.RemoveAt(index);
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

        public void RemoveAt(int index) => Frames.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() => Frames.Values.GetEnumerator();

        public TMedia? FindFrame(TimeExtent startTime)
        {
            var index = FindFrameIndex(startTime);
            return index < 0
                ? null
                : Frames[index];
        }

        public int FindFrameIndex(TimeExtent startTime)
        {
            if (IsEmpty || startTime.IsNaN)
                return -1;

            var frameIndex = 0;
            var frameTimes = Frames.Keys;
            for (var fi = 0; fi < frameTimes.Count; fi++)
            {
                if (frameTimes[fi] > startTime)
                    break;

                frameIndex = fi;
            }

            return frameIndex;
        }

        public double FindRelativePosition(TimeExtent time)
        {
            if (IsEmpty)
                return 0;

            var timeOffset = (time - MinStartTime).Milliseconds;
            return timeOffset / TotalDuration.Milliseconds;
        }

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
        public void Dispose()
        {
            Parent.CurrentLock?.Dispose();
            Parent.CurrentLock = null;
        }
    }
}
