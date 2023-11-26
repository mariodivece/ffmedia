namespace FFmpeg;

/// <summary>
/// Enumerates the constants prefixed with AV_CODEC_CAP_.
/// </summary>
[Flags]
public enum FFCodecCapabilities
{
    /// <summary>
    /// Undefined.
    /// </summary>
    None = 0,

    /// <summary>AV_CODEC_CAP_AVOID_PROBING = (1 &lt;&lt; 17)</summary>
    AvoidProbing = ffmpeg.AV_CODEC_CAP_AVOID_PROBING,

    /// <summary>AV_CODEC_CAP_CHANNEL_CONF = (1 &lt;&lt; 10)</summary>
    ChannelConf = ffmpeg.AV_CODEC_CAP_CHANNEL_CONF,

    /// <summary>AV_CODEC_CAP_DELAY = (1 &lt;&lt;  5)</summary>
    Delay = ffmpeg.AV_CODEC_CAP_DELAY,
    
    /// <summary>AV_CODEC_CAP_DR1 = (1 &lt;&lt;  1)</summary>
    Dr1 = ffmpeg.AV_CODEC_CAP_DR1,
    
    /// <summary>AV_CODEC_CAP_DRAW_HORIZ_BAND = (1 &lt;&lt;  0)</summary>
    DrawHorizontalBand = ffmpeg.AV_CODEC_CAP_DRAW_HORIZ_BAND,
    
    /// <summary>AV_CODEC_CAP_ENCODER_FLUSH = (1 &lt;&lt; 21)</summary>
    EncoderFlush = ffmpeg.AV_CODEC_CAP_ENCODER_FLUSH,
    
    /// <summary>AV_CODEC_CAP_ENCODER_RECON_FRAME = (1 &lt;&lt; 22)</summary>
    EncoderReconFrame = ffmpeg.AV_CODEC_CAP_ENCODER_RECON_FRAME,
    
    /// <summary>AV_CODEC_CAP_ENCODER_REORDERED_OPAQUE = (1 &lt;&lt; 20)</summary>
    EncoderReorderedOpaque = ffmpeg.AV_CODEC_CAP_ENCODER_REORDERED_OPAQUE,
    
    /// <summary>AV_CODEC_CAP_EXPERIMENTAL = (1 &lt;&lt;  9)</summary>
    Experimental = ffmpeg.AV_CODEC_CAP_EXPERIMENTAL,
    
    /// <summary>AV_CODEC_CAP_FRAME_THREADS = (1 &lt;&lt; 12)</summary>
    FrameThreads = ffmpeg.AV_CODEC_CAP_FRAME_THREADS,
    
    /// <summary>AV_CODEC_CAP_HARDWARE = (1 &lt;&lt; 18)</summary>
    Hardware = ffmpeg.AV_CODEC_CAP_HARDWARE,
    
    /// <summary>AV_CODEC_CAP_HYBRID = (1 &lt;&lt; 19)</summary>
    Hybrid = ffmpeg.AV_CODEC_CAP_HYBRID,
    
    /// <summary>AV_CODEC_CAP_OTHER_THREADS = (1 &lt;&lt; 15)</summary>
    OtherThreads = ffmpeg.AV_CODEC_CAP_OTHER_THREADS,
    
    /// <summary>AV_CODEC_CAP_PARAM_CHANGE = (1 &lt;&lt; 14)</summary>
    ParamChange = ffmpeg.AV_CODEC_CAP_PARAM_CHANGE,
    
    /// <summary>AV_CODEC_CAP_SLICE_THREADS = (1 &lt;&lt; 13)</summary>
    SliceThreads = ffmpeg.AV_CODEC_CAP_SLICE_THREADS,
    
    /// <summary>AV_CODEC_CAP_SMALL_LAST_FRAME = (1 &lt;&lt;  6)</summary>
    SmallLastFrame = ffmpeg.AV_CODEC_CAP_SMALL_LAST_FRAME,
    
    /// <summary>AV_CODEC_CAP_SUBFRAMES = (1 &lt;&lt;  8)</summary>
    SubFrames = ffmpeg.AV_CODEC_CAP_SUBFRAMES,
    
    /// <summary>AV_CODEC_CAP_VARIABLE_FRAME_SIZE = (1 &lt;&lt; 16)</summary>
    VariableFrameSize = ffmpeg.AV_CODEC_CAP_VARIABLE_FRAME_SIZE
}
