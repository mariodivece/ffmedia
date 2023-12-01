namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for the <see cref="AVFilterGraph"/> data structure.
/// </summary>
public unsafe sealed class FFFilterGraph :
    NativeTrackedReferenceBase<AVFilterGraph>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFFilterGraph"/> class.
    /// Allocates the unmanaged object.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFFilterGraph([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
        : base(ffmpeg.avfilter_graph_alloc(), filePath, lineNumber)
    {
        // placeholder
    }

    /// <summary>
    /// Gets a list of filters in this filter graph.
    /// </summary>
    public FFFilterSet Filters => new(this);

    /// <summary>
    /// Gets or sets the maximum number of threads used by filters in this graph.
    /// May be set by the caller before adding any filters to the filtergraph.
    /// Zero (the default) means that the number of threads is determined automatically.
    /// </summary>
    public int ThreadCount
    {
        get => Target->nb_threads;
        set => Target->nb_threads = value;
    }

    /// <summary>
    /// Gets or sets sws options to use for the auto-inserted scale filters
    /// </summary>
    public string? PictureScalerOptions
    {
        get => NativeExtensions.ReadString(Target->scale_sws_opts);
        set => Target->scale_sws_opts = value is null ? default : ffmpeg.av_strdup(value);
    }

    /// <summary>
    /// Adds a filter to the graph based on the filter prototype.
    /// </summary>
    /// <param name="filter">The filter prototype.</param>
    /// <param name="uniqueName">The unique name of this filter within this graph.</param>
    /// <param name="options">The human-readable set of options. Example: 'sample_rate=48000:channels=2'</param>
    /// <returns>The filter instance that was added.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the filter or unique name are null.</exception>
    /// <exception cref="FFException">Thrown when the filter could not be added.</exception>
    public FFFilterContext AddFilter(FFFilter filter, string uniqueName, string? options = default)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);

        if (filter is null || filter.IsNull)
            throw new ArgumentNullException(nameof(filter));

        AVFilterContext* pointer = default;
        var resultCode = ffmpeg.avfilter_graph_create_filter(
            &pointer, filter.Target, uniqueName, options, null, Target);

        return pointer is not null && resultCode >= 0
            ? new FFFilterContext(pointer)
            : throw new FFException(resultCode, $"Failed to create filter context '{uniqueName}'");
    }

    /// <summary>
    /// Adds a filter to the graph based on the filter prototype name.
    /// </summary>
    /// <param name="knownFilterName">The filter prototype name.</param>
    /// <param name="uniqueName">The unique name of this filter within this graph.</param>
    /// <param name="options">The human-readable set of options. Example: 'sample_rate=48000:channels=2'</param>
    /// <returns>The filter instance that was added.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the filter name is not found.</exception>
    public FFFilterContext AddFilter(string knownFilterName, string uniqueName, string? options = default)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(knownFilterName);
        ArgumentException.ThrowIfNullOrWhiteSpace(uniqueName);

        if (!FFFilter.TryFind(knownFilterName, out var filter))
            throw new KeyNotFoundException($"Filter with name '{nameof(knownFilterName)}' could not be found.");

        return AddFilter(filter, uniqueName, options);
    }

    /// <summary>
    /// Parses a filter literal and adds it to the filter graph.
    /// </summary>
    /// <param name="graphLiteral">The string containing the graph literal.</param>
    /// <param name="input">The input pad of the filter.</param>
    /// <param name="output">The output pad of the filter.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the arguments are null or empty.</exception>
    public void ParseLiteral(string graphLiteral, FFFilterInOut input, FFFilterInOut output)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(graphLiteral);

        if (input is null || input.IsNull)
            throw new ArgumentNullException(nameof(input));

        if (output is null || output.IsNull)
            throw new ArgumentNullException(nameof(output));

        var inputs = input.Target;
        var outputs = output.Target;
        var resultCode = ffmpeg.avfilter_graph_parse_ptr(Target, graphLiteral, &inputs, &outputs, null);
        FFException.ThrowIfNegative(resultCode, $"Could not parse filtergraph literal: {graphLiteral}");
    }

    /// <summary>
    /// Check validity and configure all the links and formats in the graph.
    /// Will throw on error.
    /// </summary>
    /// <remarks>See <see cref="ffmpeg.avfilter_graph_config"/>.</remarks>
    public void Commit()
    {
        var resultCode = ffmpeg.avfilter_graph_config(Target, null);
        FFException.ThrowIfNegative(resultCode, "Could not commit filtergraph configuration.");
    }

    /// <summary>
    /// Destroys the current filter grpah and all of its child objects and
    /// allocates a new one in this same reference.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the structure could not be allocated.</exception>
    public void Reallocate()
    {
        var target = Target;
        if (target is not null)
            ffmpeg.avfilter_graph_free(&target);

        target = ffmpeg.avfilter_graph_alloc();
        Update(target);

        if (target is null)
        {
            Dispose();
            throw new InvalidOperationException($"Could not allocate '{nameof(AVFilterGraph)}'");
        }
    }

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVFilterGraph* target) =>
        ffmpeg.avfilter_graph_free(&target);
}
