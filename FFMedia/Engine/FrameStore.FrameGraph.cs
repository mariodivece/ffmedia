namespace FFMedia.Engine;

public partial class FrameStore<TMedia>
{
    private sealed class FrameGraph : IFrameGraph<TMedia>
    {
        private readonly FrameStore<TMedia> Parent;

        public FrameGraph(FrameStore<TMedia> parent)
        {
            Parent = parent;
            // TODO: clear frames not belonging to the group index
            // TODO: Use a frame pool to return the frame
            // TODO: Expose Frame graph interface members
        }

        public int GroupIndex => Parent.GroupIndex;

        public IMediaComponent<TMedia> Component => Parent.Component;

        public SortedList<TimeExtent, TMedia> Frames => Parent.Frames;

        public bool IsEmpty => Parent.Frames.Any();

        public int MinIndex => IsEmpty ? -1 : 0;

        public int MaxIndex => IsEmpty ? -1 : Parent.Frames.Count - 1;

        public void Write(TMedia frame)
        {
            if (frame is null)
                throw new ArgumentNullException(nameof(frame));

            if (frame.StartTime.IsNaN)
                throw new ArgumentException($"{nameof(frame)}.{nameof(frame.StartTime)} has to be finite.");

            Frames.Add(frame.StartTime, frame);

        }

        private int FindFrameIndex(TimeExtent startTime)
        {
            if (IsEmpty) return -1;

            // TODO: Frame index lookup
            return 0;
        }

        public void Dispose()
        {
            Parent.CurrentLock?.Dispose();
            Parent.CurrentLock = null;
            Parent.Count = Frames.Count;
        }
    }

}
