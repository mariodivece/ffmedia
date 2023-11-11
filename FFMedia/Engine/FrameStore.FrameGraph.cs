using System.Runtime.InteropServices;

namespace FFMedia.Engine;

public partial class FrameStore<TMedia>
{
    private sealed class FrameGraph : IFrameGraph<TMedia>
    {
        private readonly FrameStore<TMedia> Parent;

        public FrameGraph(FrameStore<TMedia> parent)
        {
            Parent = parent;
            // TODO: If implemented by Frame store, write the pending frames
            // TODO: clear frames not belonging to the group index
            // TODO: Use a frame pool to return the frame
            // TODO: Expose Frame graph interface members
        }

        public int GroupIndex => Parent.GroupIndex;

        public IMediaComponent<TMedia> Component => Parent.Component;

        public bool IsFull => Frames.Count >= Parent.Capacity;

        private SortedFrameList Frames => Parent.Frames;

        public void Write(TMedia frame)
        {
            Frames.Add(frame);
        }

        public void Dispose()
        {
            Parent.CurrentLock?.Dispose();
            Parent.CurrentLock = null;
            Parent.Count = Frames.Count;
        }
    }

}
