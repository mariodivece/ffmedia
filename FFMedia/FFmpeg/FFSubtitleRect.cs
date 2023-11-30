namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVSubtitleRect"/>.
/// </summary>
public unsafe class FFSubtitleRect : 
    NativeReferenceBase<AVSubtitleRect>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFSubtitleRect"/> class.
    /// </summary>
    /// <param name="target">The wrapped data structure.</param>
    public FFSubtitleRect(AVSubtitleRect* target)
        : base(target)
    {
        // placeholder
    }

    /// <summary>
    /// Gets the subtitle type.
    /// </summary>
    public AVSubtitleType SubtitleType => Target->type;

    /// <summary>
    /// Gets the text contained in this subtitle.
    /// </summary>
    public string? PlainText => NativeExtensions.ReadString(Target->text);

    /// <summary>
    /// Gets the ASS/SSA compatible event line.
    /// The presentation of this is unaffected by the other values in this struct.
    /// </summary>
    public string? SsaText => NativeExtensions.ReadString(Target->ass);

    /// <summary>
    /// Gets the flags for this rect.
    /// </summary>
    public int Flags => Target->flags;

    /// <summary>
    /// Gets the number of color in the picture -- if any,
    /// </summary>
    public int ColorCount => HasPicture ? Target->nb_colors : 0;

    /// <summary>
    /// Gets a value specifying if this rect holds a valid subtitle image.
    /// Subtitle bitmap images are special in the sense that they
    /// are like PAL8 images. first pointer to data, second to the
    /// palette.
    /// </summary>
    public bool HasPicture =>
        SubtitleType == AVSubtitleType.SUBTITLE_BITMAP &&
        Target->data[0] is not null &&
        Target->linesize[0] > 0 &&
        Target->linesize[1] == ffmpeg.AVPALETTE_SIZE;

    /// <summary>
    /// Gets a value indicating the pixel fomat of the current rect.
    /// </summary>
    public AVPixelFormat PixelFormat => HasPicture 
        ? AVPixelFormat.AV_PIX_FMT_PAL8
        : AVPixelFormat.AV_PIX_FMT_NONE;

    /// <summary>
    /// Gets the picture data planes.
    /// </summary>
    public byte_ptr4 PictureData => Target->data;

    /// <summary>
    /// Gets the picture line sizes.
    /// </summary>
    public int4 PictureBytesPerRow => Target->linesize;

    /// <summary>
    /// Gets the X position of the rect.
    /// </summary>
    public int Left
    {
        get => Target->x;
        set => Target->x = value;
    }

    /// <summary>
    /// Gets the Y position of the rect.
    /// </summary>
    public int Top
    {
        get => Target->y;
        set => Target->y = value;
    }

    /// <summary>
    /// Gets the width of the rect.
    /// </summary>
    public int Width
    {
        get => Target->w;
        set => Target->w = value;
    }

    /// <summary>
    /// Gets the height of the rect.
    /// </summary>
    public int Height
    {
        get => Target->h;
        set => Target->h = value;
    }
}
