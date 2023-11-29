using System.Collections;

namespace FFmpeg;

/// <summary>
/// Implements a <see cref="IDictionary{TKey, TValue}"/> based on a <see cref="AVDictionary"/>.
/// </summary>
/// <remarks>
/// The <see cref="TryGetValue(string, out string)"/> tries to match as follows:
/// <br /> 1. Case sensitive, exact
/// <br /> 2. Case insensitive exact
/// <br /> 3. Case sensitive starts-with
/// <br /> 4. Case insensitive starts-with
/// </remarks>
public unsafe class FFDictionary :
    NativeTrackedReferenceBase<AVDictionary>,
    IDictionary<string, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFDictionary"/> class.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFDictionary([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        AllocateEmpty();
    }

    /// <inheritdoc />
    public string this[string key]
    {
        get
        {
            ArgumentNullException.ThrowIfNull(key);
            return TryGetValue(key, out var value)
                ? value
                : throw new KeyNotFoundException($"The specified key '{key}' was not found in the dictionary.");
        }
        set
        {
            var dictionary = Target;
            try
            {
                var result = ffmpeg.av_dict_set(&dictionary, key, value, default);
                FFException.ThrowIfNegative(result);
            }
            finally
            {
                Update(dictionary);
            }
        }
    }

    /// <inheritdoc />
    public ICollection<string> Keys => GetEntries().Select(c => c.Key).ToArray();

    /// <inheritdoc />
    public ICollection<string> Values => GetEntries().Select(c => c.Value).ToArray();

    /// <inheritdoc />
    public int Count => IsNull ? 0 : Math.Max(ffmpeg.av_dict_count(Target), 0);

    /// <inheritdoc />
    public bool IsReadOnly => false;

    /// <inheritdoc />
    public void Add(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        if (ContainsKey(key))
            throw new ArgumentException($"The key '{key}' was already present in the dictionary");

        var dictionary = Target;
        try
        {
            var result = ffmpeg.av_dict_set(&dictionary, key, value, default);
            FFException.ThrowIfNegative(result);
        }
        finally
        {
            Update(dictionary);
        }
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, string> item) =>
        Add(item.Key, item.Value);

    /// <inheritdoc />
    public void Clear() => AllocateEmpty();

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, string> item) => ContainsKey(item.Key);

    /// <inheritdoc />
    public bool ContainsKey(string key) => TryGetValue(key, out _);

    /// <inheritdoc />
    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        if (array is null || array.Length <= 0)
            return;

        var currentOffset = arrayIndex;
        foreach (var kvp in GetEntries())
        {
            if (currentOffset >= array.Length)
                break;

            array[currentOffset] = kvp.ToKeyValuePair();
            currentOffset++;
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() =>
        GetEntries().Select(c => c.ToKeyValuePair()).GetEnumerator();

    /// <inheritdoc />
    public bool Remove(string key)
    {
        if (Count <= 0)
            return false;

        var dictionary = Target;
        var result = 0;

        try
        {
            result = ffmpeg.av_dict_set(&dictionary, key, null, default);
        }
        finally
        {
            Update(dictionary);
        }
        
        return result >= 0;
    }

    /// <inheritdoc />
    public bool Remove(KeyValuePair<string, string> item) => Remove(item.Key);

    /// <inheritdoc />
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out string value)
    {
        value = null;
        if (Count <= 0) return false;
        if (string.IsNullOrWhiteSpace(key)) return false;


        var dictionary = Target;

        // Try an exact, case-sensitive match.
        var result = ffmpeg.av_dict_get(Target, key, null, ffmpeg.AV_DICT_MATCH_CASE);

        // Try an exact, case-insensitive match.
        if (result is null)
            result = ffmpeg.av_dict_get(Target, key, null, default);

        // Try an case sensitive starts-with match.
        if (result is null)
            result = ffmpeg.av_dict_get(Target, key, null, ffmpeg.AV_DICT_MATCH_CASE | ffmpeg.AV_DICT_IGNORE_SUFFIX);

        // Try an case insensitive starts with match.
        if (result is null)
            result = ffmpeg.av_dict_get(Target, key, null, ffmpeg.AV_DICT_IGNORE_SUFFIX);

        if (result is not null)
            value = NativeExtensions.ReadString(result->value);

        return value is not null;
    }

    /// <summary>
    /// Appends the given value to an existing key.
    /// If the key does not exist, then just adds the key with
    /// the specified value to the dictionary.
    /// </summary>
    /// <param name="key">The dictionary key.</param>
    /// <param name="value">The dictionary value.</param>
    public void AppendEntry(string key, string value)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(value);

        var flags = ContainsKey(key)
            ? ffmpeg.AV_DICT_APPEND
            : 0;

        var dictionary = Target;
        try
        {
            var result = ffmpeg.av_dict_set(&dictionary, key, value, flags);
            FFException.ThrowIfNegative(result);
        }
        finally
        {
            Update(dictionary);
        }
    }

    /// <summary>
    /// Iterates over the entries and returns it as a list.
    /// </summary>
    /// <returns>A list of entries currently stored in the dictionary.</returns>
    private IReadOnlyList<FFDictionaryEntry> GetEntries()
    {
        if (IsNull)
            return Array.Empty<FFDictionaryEntry>();

        var count = Math.Max(ffmpeg.av_dict_count(Target), 0);
        var result = new List<FFDictionaryEntry>(count);
        AVDictionaryEntry* currentEntry = null;

        do
        {
            currentEntry = ffmpeg.av_dict_iterate(Target, currentEntry);
            if (currentEntry is not null && currentEntry->value is not null)
                result.Add(new(currentEntry));

        } while (currentEntry is not null);

        return result;
    }

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVDictionary* target) =>
        ffmpeg.av_dict_free(&target);

    /// <summary>
    /// Releases current dictionary allocation (if allocated) and creates
    /// a new empty dictionary pointer.
    /// </summary>
    private void AllocateEmpty()
    {
        AVDictionary* dictionary = Target is null ? null : Target;

        try
        {
            if (dictionary is not null)
                ffmpeg.av_dict_free(&dictionary);

            var result = ffmpeg.av_dict_set(&dictionary, string.Empty, null, default);
            FFException.ThrowIfNegative(result);
        }
        finally
        {
            Update(dictionary);
        }
    }
}
