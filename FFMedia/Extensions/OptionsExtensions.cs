namespace FFMedia.Extensions;

public static unsafe class OptionsExtensions
{
    public static bool TryFindOption(this IFFOptionsEnabled store, string optionName,
        bool searchChildren, [MaybeNullWhen(false)] out FFOption option)
    {
        option = default;
        if (store is null || store.IsNull)
            return false;

        var searchFlags = (searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0);
        var avoption = ffmpeg.av_opt_find(store.Address.ToPointer(), optionName, null, 0, searchFlags);
        if (avoption is not null)
        {
            option = new(avoption);
            return true;
        }

        return false;
    }

    public static bool HasOption(this IFFOptionsEnabled store, string optionName, bool searchChildren) =>
        store.TryFindOption(optionName, searchChildren, out _);

    public static void SetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        ArgumentNullException.ThrowIfNull(store);
        ObjectDisposedException.ThrowIf(store.IsNull, store);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(store.Address.ToPointer(), optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    public static bool TrySetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren, string? value)
    {
        if (store is null || store.IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(store.Address.ToPointer(), optionName, value, searchFlags);
        return resultCode == 0;
    }

    public static string GetOptionValue(this IFFOptionsEnabled store, string optionName, bool searchChildren)
    {
        ArgumentNullException.ThrowIfNull(store);
        ObjectDisposedException.ThrowIf(store.IsNull, store);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(store.Address.ToPointer(), optionName, searchFlags, &stringValuePointer);

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
        var resultCode = ffmpeg.av_opt_get(store.Address.ToPointer(), optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer);
            return true;
        }

        return false;
    }

}
