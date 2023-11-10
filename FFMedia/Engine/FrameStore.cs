using System.Threading.Channels;

namespace FFMedia.Engine;

/// <summary>
/// Represents a queue-style data structure used to read and write media frames.
/// Internally, it is implemented with 2 queues. One with readable frames and
/// one with writable frames. Writable frames are leased, and then equeued on to
/// the readable frame queue. Readable frames are peeeked and then dequeued and back
/// on to the writable frames queue.
/// </summary>
public sealed class FrameStore
    //IDisposable,
    //ISerialGroupable
{
    
}
