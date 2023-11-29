namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVCodecParameters"/>.
/// </summary>
public unsafe sealed class FFCodecParameters :
    NativeReferenceBase<AVCodecParameters>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFCodecParameters"/> class.
    /// </summary>
    /// <param name="target"></param>
    public FFCodecParameters(AVCodecParameters* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>Gets the general type of the encoded data.</summary>
    public AVMediaType MediaType => Target->codec_type;

    /// <summary>Gets the specific type of the encoded data (the codec used).</summary>
    public AVCodecID CodecId => Target->codec_id;

    /// <summary>Gets additional information about the codec (corresponds to the AVI FOURCC).</summary>
    public uint CodecTag => Target->codec_tag;

    /// <summary>Gets extra binary data needed for initializing the decoder, codec-dependent.</summary>
    public byte* ExtraData => Target->extradata;

    /// <summary>Gets the size of the <see cref="ExtraData"/> content in bytes.</summary>
    public int ExtraDataSize => Target->extradata_size;

    /// <summary>
    /// Gets the pixel format if this is a video stream.
    /// </summary>
    public AVPixelFormat PixelFormat => (AVPixelFormat)Target->format;

    /// <summary>
    /// Gets the audio sample format if this is an audio stream.
    /// </summary>
    public AVSampleFormat SampleFormat => (AVSampleFormat)Target->format;

    /// <summary>Gets the average bitrate of the encoded data (in bits per second).</summary>
    public long BitRate => Target->bit_rate;

    /// <summary>Gets the number of bits per sample in the codedwords.</summary>
    public int BitsPerCodedSample => Target->bits_per_coded_sample;

    /// <summary>
    /// This is the number of valid bits in each output sample.
    /// If the sample format has more bits, the least significant bits are
    /// additional padding bits, which are always 0. Use right shifts to
    /// reduce the sample to its actual size. For example, audio formats
    /// with 24 bit samples will have bits_per_raw_sample set to 24,
    /// and format set to AV_SAMPLE_FMT_S32. To get the original 
    /// sample use &quot;(int32_t)sample &gt;&gt; 8&quot;.&quot;
    /// </summary>
    public int BitsPerRawSample => Target->bits_per_raw_sample;

    /// <summary>Gets codec-specific bitstream profile that the stream conforms to.</summary>
    public int BitsreamProfile => Target->profile;

    /// <summary>Gets codec-specific bitstream level that the stream conforms to.</summary>
    public int BitstreamLevel => Target->level;

    /// <summary>Gets the video pixel width.</summary>
    public int PixelWidth => Target->width;

    /// <summary>Gets the video pixel height.</summary>
    public int PixelHeight => Target->height;

    /// <summary>
    /// Gets the pixel aspect ratio (width / height) which a
    /// single pixel should have when displayed (video only).
    /// </summary>
    public AVRational PixelAspectRatio => Target->sample_aspect_ratio;

    /// <summary>Gets the order of the fields in interlaced video.</summary>
    public AVFieldOrder FieldOrder => Target->field_order;

    /// <summary>
    /// Gets the color range (video only).
    /// </summary>
    public AVColorRange ColorRange => Target->color_range;

    /// <summary>
    /// Gets the color primaries (video only).
    /// </summary>
    public AVColorPrimaries ColorPrimaries => Target->color_primaries;

    /// <summary>
    /// Gets the color transfer characteristic (video only).
    /// </summary>
    public AVColorTransferCharacteristic ColorTransfer => Target->color_trc;

    /// <summary>
    /// Gets the colorspace (video only).
    /// </summary>
    public AVColorSpace ColorSpace => Target->@color_space;

    /// <summary>
    /// Gets the location of chroma samples (video only).
    /// </summary>
    public AVChromaLocation ChromaLocation => Target->chroma_location;

    /// <summary>Gets the number of delayed frames.</summary>
    public int VideoDelayFrameCount => Target->video_delay;

    /// <summary>Gets the number of audio channels (.</summary>
    public int ChannelCount => ChannelLayout.nb_channels;
    
    /// <summary>Gets the number of audio samples per second.</summary>
    public int SampleRate => Target->sample_rate;
    
    /// <summary>The number of bytes per coded audio frame, required by some formats.</summary>
    /// <remarks><see cref="AVCodecParameters.block_align"/></remarks>
    public int BytesPerCodedFrame => Target->block_align;

    /// <summary>Gets the audio frame size, if known. Required by some formats to be static.</summary>
    /// <remarks><see cref="AVCodecParameters.frame_size"/></remarks>
    public int BytesPerFrame => Target->frame_size;
    
    /// <summary>
    /// Audio only. The amount of padding (in samples) inserted by the
    /// encoder at the beginning of the audio. I.e. this number of leading
    /// decoded samples must be discarded by the caller to get the original
    /// audio without leading padding.
    /// </summary>
    public int InitialPaddingSampleCount => Target->initial_padding;
    
    /// <summary>
    /// Audio only. The amount of padding (in samples) appended by
    /// the encoder to the end of the audio. I.e. this number of decoded samples
    /// must be discarded by the caller from the end of the stream to get the
    /// original audio without any trailing padding.
    /// </summary>
    public int TrailingPaddingSampleCount => Target->@trailing_padding;
    
    /// <summary>Number of samples to skip after a discontinuity (audio only).</summary>
    /// <remarks><see cref="AVCodecParameters.seek_preroll"/></remarks>
    public int PrerollSampleCount => Target->seek_preroll;
    
    /// <summary>Gets the the channel layout and number of channels (audio only).</summary>
    public AVChannelLayout ChannelLayout => Target->ch_layout;
}
