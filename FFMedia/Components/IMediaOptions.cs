namespace FFMedia.Components;

/// <summary>
/// Represents a set of options for a media container.
/// </summary>
public interface IMediaOptions
{
    /// <summary>
    /// Attempts to read an option value.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="value">The output value.</param>
    /// <returns>True if success, false otherise.</returns>
    bool TryGetOptionValue<T>(string key, out T? value);

    /// <summary>
    /// Attempts to read a value from the options object or returns the default.
    /// </summary>
    /// <typeparam name="T">The type of the value to retrieve.</typeparam>
    /// <param name="defaultValue">The default value to return if the value is not found.</param>
    /// <param name="propertyName">The name of the property/key to read.</param>
    /// <returns>The stored value or the default value.</returns>
    T? GetValueOrDefault<T>(T defaultValue, [CallerMemberName] string? propertyName = default);

    /// <summary>
    /// Sets a value for the specific ptoperty/key.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to set.</param>
    /// <param name="propertyName">The name of the property/key.</param>
    void SetOptionValue<T>(T value, [CallerMemberName] string? propertyName = default);

    /// <summary>
    /// Gets a value indicating whether the options object has the specified key.
    /// </summary>
    /// <param name="key">The key name.</param>
    /// <returns>True if the key exists. False otherwise.</returns>
    bool ContainsKey(string key);

    /// <summary>
    /// Gets a collection of keys currently stored in the options object.
    /// </summary>
    ICollection<string> Keys { get; }

    /// <summary>
    /// Casts the internal dictionary as a dynamic object.
    /// </summary>
    /// <returns>The dynamically-casted object.</returns>
    dynamic AsDynamic();

    /// <summary>
    /// Casts the internal dictionary as a set of
    /// key-value pairs.
    /// </summary>
    /// <returns>The internal dictionary.</returns>
    IDictionary<string, object?> AsDictionary();
}
