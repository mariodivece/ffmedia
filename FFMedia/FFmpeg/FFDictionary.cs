using System.Collections;

namespace FFmpeg;

/// <summary>
/// Implements a <see cref="IDictionary{TKey, TValue}"/> based on a <see cref="AVDictionary"/>.
/// The key lookups are not case-sensitive.
/// </summary>
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
        AVDictionary* dictionary = null;
        ffmpeg.av_dict_set(&dictionary, null, null, default);
        Update(dictionary);
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
        set => Add(key, value);
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

        var dictionary = Target;
        var result = ffmpeg.av_dict_set(&dictionary, key, value, default);
        Update(dictionary);
        FFException.ThrowIfNegative(result);
    }

    /// <inheritdoc />
    public void Add(KeyValuePair<string, string> item) =>
        Add(item.Key, item.Value);

    /// <inheritdoc />
    public void Clear()
    {
        var dictionary = Target;

        if (dictionary is not null)
        ffmpeg.av_dict_free(&dictionary);
        ffmpeg.av_dict_set(&dictionary, null, null, default);
        Update(dictionary);
    }

    /// <inheritdoc />
    public bool Contains(KeyValuePair<string, string> item) => ContainsKey(item.Key);

    /// <inheritdoc />
    public bool ContainsKey(string key)
    {
        if (IsNull)
            return false;

        var entry = ffmpeg.av_dict_get(Target, key, null, default);
        return entry is not null && entry->value is not null;
    }

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
        var result = ffmpeg.av_dict_set(&dictionary, key, null, default);
        Update(dictionary);
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
        var result = ffmpeg.av_dict_get(Target, key, null, default);

        if (result is not null)
            value = NativeExtensions.ReadString(result->value);

        return value is not null;
    }

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
}
