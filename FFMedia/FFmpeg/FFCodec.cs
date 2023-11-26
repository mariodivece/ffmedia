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
    public AVCodecID Id => Target->id;

    /// <summary>
    /// Gets a value indicating that the codec supports encoding.
    /// </summary>
    public bool IsEncoder => ffmpeg.av_codec_is_encoder(Target).ToBool();

    /// <summary>
    /// Gets a value indication that the codec supports decoding. 
    /// </summary>
    public bool IsDecoder => ffmpeg.av_codec_is_decoder(Target).ToBool();

    /// <summary>
    /// Gets the codec's media type.
    /// </summary>
    public AVMediaType MediaType => Target->type;

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
    public FFMediaClass PrivateMediaClass => new(Target->priv_class);

    /// <summary>
    /// Gets the maximum low resolution factor for the decoder.
    /// </summary>
    public int MaxLowResFactor => Target->max_lowres;

    /// <summary>
    /// Gets the codec name.
    /// </summary>
    public string Name => NativeExtensions.ReadString(Target->name) ?? string.Empty;

    /// <summary>
    /// Gets the human=readable, friendly codec name.
    /// </summary>
    public string LongName => NativeExtensions.ReadString(Target->long_name) ?? string.Empty;

    /// <summary>
    /// Gets the group name of the codec implementation.
    /// This is a short symbolic name of the wrapper backing this codec.
    /// A wrapper uses some kind of external implementation for the codec, such as an external
    /// library, or a codec implementation provided by the OS or the hardware.
    /// If this field is null, this is a builtin, libavcodec native codec.
    /// If non-NULL, this will be the suffix in AVCodec.name in most cases.
    /// </summary>
    public string? GroupName => NativeExtensions.ReadString(Target->wrapper_name);

    /// <summary>
    /// Gets a list of supported pixel formats.
    /// </summary>
    public IReadOnlyList<AVPixelFormat> PixelFormats =>
        NativeExtensions.ExtractArray(Target->pix_fmts, AVPixelFormat.AV_PIX_FMT_NONE);

    /// <summary>
    /// Gets a list of supported frame rates.
    /// </summary>
    public IReadOnlyList<AVRational> FrameRates =>
        NativeExtensions.ExtractArray(Target->supported_framerates, default);

    /// <summary>
    /// Gets a list of supported sample formats.
    /// </summary>
    public IReadOnlyList<AVSampleFormat> SampleFormats =>
        NativeExtensions.ExtractArray(Target->sample_fmts, AVSampleFormat.AV_SAMPLE_FMT_NONE);

    /// <summary>
    /// Gets a list of supported sample rates.
    /// </summary>
    public IReadOnlyList<int> SampleRates =>
        NativeExtensions.ExtractArray(Target->supported_samplerates, 0);

    /// <summary>
    /// Gets a list of supported channel layouts.
    /// </summary>
    public IReadOnlyList<AVChannelLayout> ChannelLayouts =>
        NativeExtensions.ExtractArray(Target->ch_layouts, default);

    /// <summary>
    /// Gets a list of supported profiles.
    /// </summary>
    public IReadOnlyList<AVProfile> Profiles =>
        NativeExtensions.ExtractArray(Target->profiles, new() { profile = ffmpeg.FF_PROFILE_UNKNOWN });

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
