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

    /// <summary>
    /// Gets an emprty reference to codec parameters.
    /// </summary>
    public static FFCodecParameters Empty { get; } = new(null);

    /// <summary>Gets the general type of the encoded data.</summary>
    public AVMediaType MediaType => Target is not null ? Target->codec_type : AVMediaType.AVMEDIA_TYPE_UNKNOWN;

    /// <summary>Gets the specific type of the encoded data (the codec used).</summary>
    public AVCodecID CodecId => Target is not not null ? Target->codec_id : AVCodecID.AV_CODEC_ID_NONE;

    /// <summary>Gets additional information about the codec (corresponds to the AVI FOURCC).</summary>
    public uint CodecTag => Target is not null ? Target->codec_tag : default;

    /// <summary>Gets extra binary data needed for initializing the decoder, codec-dependent.</summary>
    public byte* ExtraData => Target is not null ? Target->extradata : default;

    /// <summary>Gets the size of the <see cref="ExtraData"/> content in bytes.</summary>
    public int ExtraDataSize => Target is not null ? Target->extradata_size : default;

    /// <summary>
    /// Gets the pixel format if this is a video stream.
    /// </summary>
    public AVPixelFormat PixelFormat => Target is not null ? (AVPixelFormat)Target->format : AVPixelFormat.AV_PIX_FMT_NONE;

    /// <summary>
    /// Gets the audio sample format if this is an audio stream.
    /// </summary>
    public AVSampleFormat SampleFormat => Target is not null ? (AVSampleFormat)Target->format : AVSampleFormat.AV_SAMPLE_FMT_NONE;

    /// <summary>Gets the average bitrate of the encoded data (in bits per second).</summary>
    public long BitRate => Target is not null ? Target->bit_rate : default;

    /// <summary>Gets the number of bits per sample in the codedwords.</summary>
    public int BitsPerCodedSample => Target is not null ? Target->bits_per_coded_sample : default;

    /// <summary>
    /// This is the number of valid bits in each output sample.
    /// If the sample format has more bits, the least significant bits are
    /// additional padding bits, which are always 0. Use right shifts to
    /// reduce the sample to its actual size. For example, audio formats
    /// with 24 bit samples will have bits_per_raw_sample set to 24,
    /// and format set to AV_SAMPLE_FMT_S32. To get the original 
    /// sample use &quot;(int32_t)sample &gt;&gt; 8&quot;.&quot;
    /// </summary>
    public int BitsPerRawSample => Target is not null ? Target->bits_per_raw_sample : default;

    /// <summary>Gets codec-specific bitstream profile that the stream conforms to.</summary>
    public int BitsreamProfile => Target is not null ? Target->profile : default;

    /// <summary>Gets codec-specific bitstream level that the stream conforms to.</summary>
    public int BitstreamLevel => Target is not null ? Target->level : default;

    /// <summary>Gets the video pixel width.</summary>
    public int PixelWidth => Target is not null ? Target->width : default;

    /// <summary>Gets the video pixel height.</summary>
    public int PixelHeight => Target is not null ? Target->height : default;

    /// <summary>
    /// Gets the pixel aspect ratio (width / height) which a
    /// single pixel should have when displayed (video only).
    /// </summary>
    public AVRational PixelAspectRatio => Target is not null
        ? Target->sample_aspect_ratio
        : RationalExtensions.UndefinedValue;

    /// <summary>Gets the order of the fields in interlaced video.</summary>
    public AVFieldOrder FieldOrder => Target is not null
        ? Target->field_order
        : AVFieldOrder.AV_FIELD_UNKNOWN;

    /// <summary>
    /// Gets the color range (video only).
    /// </summary>
    public AVColorRange ColorRange => Target is not null
        ? Target->color_range
        : AVColorRange.AVCOL_RANGE_UNSPECIFIED;

    /// <summary>
    /// Gets the color primaries (video only).
    /// </summary>
    public AVColorPrimaries ColorPrimaries => Target is not null
        ? Target->color_primaries
        : AVColorPrimaries.AVCOL_PRI_UNSPECIFIED;

    /// <summary>
    /// Gets the color transfer characteristic (video only).
    /// </summary>
    public AVColorTransferCharacteristic ColorTransfer => Target is not null
        ? Target->color_trc
        : AVColorTransferCharacteristic.AVCOL_TRC_UNSPECIFIED;

    /// <summary>
    /// Gets the colorspace (video only).
    /// </summary>
    public AVColorSpace ColorSpace => Target is not null
        ? Target->@color_space
        : AVColorSpace.AVCOL_SPC_UNSPECIFIED;

    /// <summary>
    /// Gets the location of chroma samples (video only).
    /// </summary>
    public AVChromaLocation ChromaLocation => Target is not null
        ? Target->chroma_location 
        : AVChromaLocation.AVCHROMA_LOC_UNSPECIFIED;

    /// <summary>Gets the number of delayed frames.</summary>
    public int VideoDelayFrameCount => Target is not null ? Target->video_delay : default;

    /// <summary>Gets the number of audio channels (.</summary>
    public int ChannelCount => ChannelLayout.nb_channels;
    
    /// <summary>Gets the number of audio samples per second.</summary>
    public int SampleRate => Target is not null ? Target->sample_rate : default;
    
    /// <summary>The number of bytes per coded audio frame, required by some formats.</summary>
    /// <remarks><see cref="AVCodecParameters.block_align"/></remarks>
    public int BytesPerCodedFrame => Target is not null ? Target->block_align : default;

    /// <summary>Gets the audio frame size, if known. Required by some formats to be static.</summary>
    /// <remarks><see cref="AVCodecParameters.frame_size"/></remarks>
    public int BytesPerFrame => Target is not null ? Target->frame_size : default;
    
    /// <summary>
    /// Audio only. The amount of padding (in samples) inserted by the
    /// encoder at the beginning of the audio. I.e. this number of leading
    /// decoded samples must be discarded by the caller to get the original
    /// audio without leading padding.
    /// </summary>
    public int InitialPaddingSampleCount => Target is not null ? Target->initial_padding : default;
    
    /// <summary>
    /// Audio only. The amount of padding (in samples) appended by
    /// the encoder to the end of the audio. I.e. this number of decoded samples
    /// must be discarded by the caller from the end of the stream to get the
    /// original audio without any trailing padding.
    /// </summary>
    public int TrailingPaddingSampleCount => Target is not null ? Target->trailing_padding : default;
    
    /// <summary>Number of samples to skip after a discontinuity (audio only).</summary>
    /// <remarks><see cref="AVCodecParameters.seek_preroll"/></remarks>
    public int PrerollSampleCount => Target is not null ? Target->seek_preroll : default;
    
    /// <summary>Gets the the channel layout and number of channels (audio only).</summary>
    public AVChannelLayout ChannelLayout => Target is not null ? Target->ch_layout : default;
}
