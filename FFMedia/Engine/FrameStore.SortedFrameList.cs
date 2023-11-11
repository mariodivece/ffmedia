using System.Collections;

namespace FFMedia.Engine;

public partial class FrameStore<TMedia>
{
    private sealed class SortedFrameList : IList<TMedia>
    {
        private readonly SortedList<TimeExtent, TMedia> Frames;

        public SortedFrameList(int initialCapacity)
        {
            Frames = new(initialCapacity);
        }

        public TMedia this[int index]
        {
            get => Frames.GetValueAtIndex(index);
            set => throw new NotSupportedException($"Setting an item at an index of a '{nameof(SortedFrameList)}' defeats its purpose.");
        }

        public int Count => Frames.Count;

        public bool IsReadOnly => false;

        public bool IsEmpty => Frames.Any();

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

        public void Add(TMedia item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.StartTime.IsNaN)
                throw new ArgumentException($"{nameof(item)}.{nameof(item.StartTime)} has to be finite.");

            if (item.Duration.IsNaN || item.Duration < TimeExtent.Zero)
                throw new ArgumentException($"{nameof(item)}.{nameof(item.Duration)} has to be finite and non-negative.");

            if (Frames.ContainsValue(item))
                throw new ArgumentException($"{nameof(item)} is already contained in the set.");

            if (Frames.ContainsKey(item.StartTime))
                throw new ArgumentException($"An {nameof(item)} with the same {nameof(item.StartTime)} is already contained in the set.");

            Frames.Add(item.StartTime, item);
        }

        public void Clear() => Frames.Clear();

        public bool Contains(TMedia item) => Frames.ContainsValue(item);

        public void CopyTo(TMedia[] array, int arrayIndex) => Frames.Values.CopyTo(array, arrayIndex);

        public IEnumerator<TMedia> GetEnumerator() => Frames.Values.GetEnumerator();

        public int IndexOf(TMedia item) => Frames.IndexOfValue(item);

        public void Insert(int index, TMedia item) =>
            throw new NotSupportedException($"Inserting an item at an index of a '{nameof(SortedFrameList)}' defeats its purpose.");

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

        /// <summary>
        /// Finds the relative position of the specified time within
        /// <see cref="MinStartTime"/> and <see cref="RangeEndTime"/>.
        /// Negative numbers: the time occurs before the first position.
        /// Numbers greater than 1: the time occurs after the last position.
        /// Numbers 0.0 to 1.0: the time occurs at the given percent of the
        /// <see cref="TotalDuration"/>. If <see cref="IsEmpty"/> evaluates to true,
        /// then the output will be 0.0.
        /// </summary>
        /// <param name="time">The time to compute the relative position for.</param>
        /// <returns>The relative position of the given time</returns>
        public double FindPosition(TimeExtent time)
        {
            if (IsEmpty)
                return 0;

            var timeOffset = (time - MinStartTime).Milliseconds;
            return timeOffset / TotalDuration.Milliseconds;
        }

        public double FindPosition(TMedia item)
        {
            if (item is null)
                throw new ArgumentNullException(nameof(item));

            if (item.StartTime.IsNaN)
                throw new ArgumentException($"{nameof(item)}.{nameof(item.StartTime)} has to be finite.");

            if (item.Duration.IsNaN || item.Duration < TimeExtent.Zero)
                throw new ArgumentException($"{nameof(item)}.{nameof(item.Duration)} has to be finite and non-negative.");

            return FindPosition(item.StartTime + (item.Duration / 2d));
        }
    }
}
