namespace FFmpeg;

/// <summary>
/// Wraps a <see cref="AVCodec"/> data structure.
/// </summary>
public unsafe sealed class FFCodec : NativeReferenceBase<AVCodec>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFCodec"/> class.
    /// </summary>
    /// <param name="target">The target pointer.</param>
    public FFCodec(AVCodec* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Gets a list of registered codecs.
    /// </summary>
    public static IReadOnlyList<FFCodec> Codecs
    {
        get
        {
            var result = new List<FFCodec>(128);

            void* iterator = null;
            AVCodec* currentItem;

            do
            {
                currentItem = ffmpeg.av_codec_iterate(&iterator);
                if (currentItem is not null)
                    result.Add(new(currentItem));

            } while (currentItem is not null);

            return result;
        }
    }

    /// <summary>
    /// Gets the coded Id.
    /// </summary>
    public AVCodecID Id => Target is not null
        ? Target->id
        : AVCodecID.AV_CODEC_ID_NONE;

    /// <summary>
    /// Gets a value indicating that the codec supports encoding.
    /// </summary>
    public bool IsEncoder => Target is not null &&
        ffmpeg.av_codec_is_encoder(Target).ToBool();

    /// <summary>
    /// Gets a value indication that the codec supports decoding. 
    /// </summary>
    public bool IsDecoder => Target is not null &&
        ffmpeg.av_codec_is_decoder(Target).ToBool();

    /// <summary>
    /// Gets the codec's media type.
    /// </summary>
    public AVMediaType MediaType => Target is not null ? Target->type : AVMediaType.AVMEDIA_TYPE_UNKNOWN;

    /// <summary>
    /// Gets the codec capabilities in the form of flags.
    /// see AV_CODEC_CAP_*
    /// </summary>
    public int CapabilityFlags => IsNull ? default : Target->capabilities;

    /// <summary>
    /// Gets a list of codec capabilities extracted from its <see cref="CapabilityFlags"/>
    /// from well-known values.
    /// </summary>
    public IReadOnlyList<FFCodecCapabilities> Capabilities
    {
        get
        {
            var flags = (FFCodecCapabilities)CapabilityFlags;
            return Enum.GetValues<FFCodecCapabilities>()
                .Where(c => c != FFCodecCapabilities.None && flags.HasFlag(c))
                .ToArray();
        }
    }

    /// <summary>
    /// Gets the private <see cref="AVClass"/> for the context.
    /// </summary>
    public FFMediaClass PrivateMediaClass => Target is not null && Target->priv_class is not null
        ? new(Target->priv_class)
        : FFMediaClass.Empty;

    /// <summary>
    /// Gets the maximum low resolution factor for the decoder.
    /// </summary>
    public int MaxLowResFactor => Target is not null ? Target->max_lowres : default;

    /// <summary>
    /// Gets the codec name.
    /// </summary>
    public string Name => Target is not null
        ? NativeExtensions.ReadString(Target->name) ?? string.Empty
        : string.Empty;

    /// <summary>
    /// Gets the human=readable, friendly codec name.
    /// </summary>
    public string LongName => Target is not null
        ? NativeExtensions.ReadString(Target->long_name) ?? string.Empty
        : string.Empty;

    /// <summary>
    /// Gets the group name of the codec implementation.
    /// This is a short symbolic name of the wrapper backing this codec.
    /// A wrapper uses some kind of external implementation for the codec, such as an external
    /// library, or a codec implementation provided by the OS or the hardware.
    /// If this field is null, this is a builtin, libavcodec native codec.
    /// If non-NULL, this will be the suffix in AVCodec.name in most cases.
    /// </summary>
    public string? GroupName => Target is not null
        ? NativeExtensions.ReadString(Target->wrapper_name)
        : default;

    /// <summary>
    /// Gets a list of supported pixel formats.
    /// </summary>
    public IReadOnlyList<AVPixelFormat> PixelFormats => Target is not null
        ? NativeExtensions.ExtractArray(Target->pix_fmts, AVPixelFormat.AV_PIX_FMT_NONE)
        : Array.Empty<AVPixelFormat>();

    /// <summary>
    /// Gets a list of supported frame rates.
    /// </summary>
    public IReadOnlyList<AVRational> FrameRates => Target is not null
        ? NativeExtensions.ExtractArray(Target->supported_framerates, default)
        : Array.Empty<AVRational>();

    /// <summary>
    /// Gets a list of supported sample formats.
    /// </summary>
    public IReadOnlyList<AVSampleFormat> SampleFormats => Target is not null
        ? NativeExtensions.ExtractArray(Target->sample_fmts, AVSampleFormat.AV_SAMPLE_FMT_NONE)
        : Array.Empty<AVSampleFormat>();

    /// <summary>
    /// Gets a list of supported sample rates.
    /// </summary>
    public IReadOnlyList<int> SampleRates => Target is not null
        ? NativeExtensions.ExtractArray(Target->supported_samplerates, 0)
        : Array.Empty<int>();

    /// <summary>
    /// Gets a list of supported channel layouts.
    /// </summary>
    public IReadOnlyList<AVChannelLayout> ChannelLayouts => Target is not null
        ? NativeExtensions.ExtractArray(Target->ch_layouts, default)
        : Array.Empty<AVChannelLayout>();

    /// <summary>
    /// Gets a list of supported profiles.
    /// </summary>
    public IReadOnlyList<AVProfile> Profiles => Target is not null
        ? NativeExtensions.ExtractArray(Target->profiles, new() { profile = ffmpeg.FF_PROFILE_UNKNOWN })
        : Array.Empty<AVProfile>();

    /// <summary>
    /// Gets the codec given the codec id.
    /// </summary>
    /// <param name="codecId">The codec id.</param>
    /// <returns>The codec, if found.</returns>
    public static FFCodec? FromDecoderId(AVCodecID codecId)
    {
        var pointer = ffmpeg.avcodec_find_decoder(codecId);
        return pointer is not null ? new(pointer) : default;
    }

    /// <summary>
    /// Gets the codec given the codec id.
    /// </summary>
    /// <param name="codecId">The codec id.</param>
    /// <returns>The codec, if found.</returns>
    public static FFCodec? FromEncoderId(AVCodecID codecId)
    {
        var pointer = ffmpeg.avcodec_find_encoder(codecId);
        return pointer is not null ? new(pointer) : default;
    }

    /// <summary>
    /// Gets the codec given the codec name.
    /// </summary>
    /// <param name="codecName">The codec name.</param>
    /// <returns>The codec, if found.</returns>
    public static FFCodec? FromDecoderName(string codecName)
    {
        var pointer = ffmpeg.avcodec_find_decoder_by_name(codecName);
        return pointer is not null ? new(pointer) : default;
    }

    /// <summary>
    /// Gets the codec given the codec name.
    /// </summary>
    /// <param name="codecName">The codec name.</param>
    /// <returns>The codec, if found.</returns>
    public static FFCodec? FromEncoderName(string codecName)
    {
        var pointer = ffmpeg.avcodec_find_encoder_by_name(codecName);
        return pointer is not null ? new(pointer) : default;
    }
}
