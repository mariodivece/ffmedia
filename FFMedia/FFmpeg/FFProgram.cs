namespace FFmpeg;

/// <summary>
/// Serves as a wrapper for the <see cref="AVProgram"/> data structure.
/// </summary>
public unsafe sealed class FFProgram : 
    NativeReferenceBase<AVProgram>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFProgram"/> class.
    /// </summary>
    /// <param name="target"></param>
    public FFProgram(AVProgram* target)
        : base(target)
    {
        ArgumentNullException.ThrowIfNull(target);
    }

    /// <summary>
    /// Gets the program id (undocumented).
    /// </summary>
    public int ProgramId => Target->id;

    /// <summary>
    /// Gets the program number (undocumented).
    /// </summary>
    public int ProgramNumber => Target->program_num;

    /// <summary>
    /// Gets the program flags (undocumented).
    /// </summary>
    public int ProgramFlags => Target->flags;

    /// <summary>
    /// Gets or sets flags to determine which program to 
    /// discard and which to feed to the caller
    /// </summary>
    public AVDiscard DiscardFlags
    {
        get => Target->discard;
        set => Target->discard = value;
    }

    /// <summary>
    /// Gets a dictionary of string key-value pairs of metadata.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata =>
        FFDictionary.ToDictionary(Target->metadata);

    /// <summary>
    /// Gets a list of related stream indices.
    /// </summary>
    public IReadOnlyList<int> StreamIndices
    {
        get
        {
            var streamCount = Convert.ToInt32(Target->nb_stream_indexes);
            var result = new List<int>(streamCount);
            for (var i = 0; i < streamCount; i++)
                result.Add(Convert.ToInt32(Target->stream_index[i]));

            return result;
        }
    }
}
