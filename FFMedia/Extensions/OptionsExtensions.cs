namespace FFMedia.Extensions;

/// <summary>
/// Provides options management for options-enabled objects as extension methods,
/// rather than having to implement each interface member individually for each
/// options-enabled class.
/// </summary>
public static unsafe class OptionsExtensions
{
    /// <summary>
    /// Attempts to find an existing option in the options-enabled object.
    /// </summary>
    /// <remarks>
    /// If search children is set to true, then the lookup occurs in the chilren first.
    /// Otherwise, the lookup occurs in the current object exclusively.
    /// </remarks>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The name of the option to look for.</param>
    /// <param name="searchChildren">The the option name is searched for in the child objects first.</param>
    /// <param name="option">The option if it was found.</param>
    /// <returns>True if the option lookup succeeds. False otherwise.</returns>
    public static bool TryFindOption(this IFFOptionsEnabled store, string optionName,
        bool searchChildren, [MaybeNullWhen(false)] out FFOption option)
    {
        option = default;
        if (store is null || store.IsNull)
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0;
        var avoption = ffmpeg.av_opt_find(store.ToPointer(), optionName, null, 0, searchFlags);
        if (avoption is not null)
        {
            option = new(avoption);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Determines if the option with the given name exists in either
    /// the options-enabled object (or in one of its children).
    /// </summary>
    /// <remarks>
    /// If search children is set to true, then the lookup occurs in the chilren first.
    /// Otherwise, the lookup occurs in the current object exclusively.
    /// </remarks>
    /// <param name="store">The options-enabled object.</param>
    /// <param name="optionName">The name of the option to look for.</param>
    /// <param name="searchChildren">The the option name is searched for in the child objects first.</param>
    /// <returns>Whether or not the option exists.</returns>
    public static bool HasOption(this IFFOptionsEnabled store, string optionName, bool searchChildren) =>
        store.TryFindOption(optionName, searchChildren, out _);

    public static void SetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        ArgumentNullException.ThrowIfNull(store);
        ObjectDisposedException.ThrowIf(store.IsNull, store);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(store.ToPointer(), optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    public static bool TrySetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(store.ToPointer(), optionName, value, searchFlags);
        return resultCode == 0;
    }

    public static string GetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren)
    {
        ArgumentNullException.ThrowIfNull(store);
        ObjectDisposedException.ThrowIf(store.IsNull, store);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(store.ToPointer(), optionName, searchFlags, &stringValuePointer);

        FFException.ThrowIfNegative(resultCode, $"Failed to get option value for '{optionName}'.");
        return NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
    }

    public static bool TryGetOptionValue(this IFFOptionsEnabled store, string optionName,
        bool searchChildren, [MaybeNullWhen(false)] out string? value)
    {
        value = null;
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(store.ToPointer(), optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer);
            return true;
        }

        return false;
    }

}
