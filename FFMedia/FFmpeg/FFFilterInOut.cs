namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for <see cref="AVFilterInOut"/>.
/// </summary>
public unsafe sealed class FFFilterInOut :
    NativeReferenceBase<AVFilterInOut>
{
    /// <summary>
    /// Creates a new instance of the <see cref="FFFilterInOut"/> class.
    /// </summary>
    /// <param name="pointer"></param>
    private FFFilterInOut(AVFilterInOut* pointer)
        : base(pointer)
    {
        // placeholder
    }

    /// <summary>
    /// Gets the unique name for this IO in the list.
    /// </summary>
    public string? Name
    {
        get => NativeExtensions.ReadString(Target->name);
        set => Target->name = value is not null ? ffmpeg.av_strdup(value) : default;
    }

    /// <summary>
    /// GEts or sets the pad index of the filter IO.
    /// </summary>
    public int PadIndex
    {
        get => Target->pad_idx;
        set => Target->pad_idx = value;
    }

    /// <summary>
    /// Gets or sets the next filter as linked by the filtergraph.
    /// </summary>
    public FFFilterInOut? Next
    {
        get => !IsNull && Target->next is not null ? new(Target->next) : default;
        set => Target->next = value is not null && !value.IsNull ? value.Target : default;
    }

    /// <summary>
    /// Gets or sets the filter context.
    /// </summary>
    public FFFilterContext? FilterContext
    {
        get => !IsNull && Target->filter_ctx is not null ? new(Target->filter_ctx) : default;
        set => Target->filter_ctx = value is not null && !value.IsNull ? value.Target : default;
    }

    /// <summary>
    /// Manually allocates an unmanaged instance of the <see cref="AVFilterInOut"/>
    /// structure. If linking to a filtergraph fails, you are responsible for
    /// calling the <see cref="Release(FFFilterInOut)"/> method manually.
    /// </summary>
    /// <returns>The newly-allocated data structure wrapper.</returns>
    public static FFFilterInOut Allocate() => new(ffmpeg.avfilter_inout_alloc());

    /// <summary>
    /// Manually releases the allocated filter in-out structure.
    /// Please see <see cref="Allocate"/> for details.
    /// </summary>
    /// <param name="target">The target wrapper that points to the allocated pointer.</param>
    public static void Release(FFFilterInOut target)
    {
        if (target is null || target.IsNull)
            return;

        var pointer = target.Target;
        ffmpeg.avfilter_inout_free(&pointer);
        target.Update(null);
    }
}
