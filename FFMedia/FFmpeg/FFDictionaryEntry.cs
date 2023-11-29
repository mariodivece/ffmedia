namespace FFmpeg;

/// <summary>
/// Represents a wrapper for a <see cref="AVDictionaryEntry"/>.
/// </summary>
public unsafe class FFDictionaryEntry  : NativeReferenceBase<AVDictionaryEntry>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FFDictionaryEntry"/>.
    /// </summary>
    /// <param name="target">The address pointer of the entry.</param>
    public FFDictionaryEntry(AVDictionaryEntry* target)
        : base(target)
    {
        // placeholder.
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    public string Key => IsNull ? string.Empty : NativeExtensions.ReadString(Target->key) ?? string.Empty;

    /// <summary>
    /// Gets the value.
    /// </summary>
    public string Value => IsNull ? string.Empty : NativeExtensions.ReadString(Target->value) ?? string.Empty;

    /// <summary>
    /// Converts the <see cref="FFDictionaryEntry"/> to a <see cref="KeyValuePair{TKey, TValue}"/>.
    /// </summary>
    /// <returns></returns>
    public KeyValuePair<string, string> ToKeyValuePair() => new(Key, Value);
}
