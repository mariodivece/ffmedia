namespace FFMedia.FFmpeg;

/// <summary>
/// Represents a wrapper for  data structures with AV-options enabled functionality.
/// Such structures must have a <see cref="AVClass"/> pointer as their first defined element.
/// This is a proxy class for implementing the <see cref="IFFOptionsEnabled"/> interface.
/// </summary>
public sealed unsafe class FFOptionsWrapper :
    NativeReference
{
    private static readonly Dictionary<Type, bool> MediaClassField = new(16);
    private static readonly object SyncRoot = new();

    /// <summary>
    /// Creates an instance of the <see cref="FFOptionsWrapper"/> class.
    /// Use the <see cref="TryWrap{T}(INativeReference{T}, out FFOptionsWrapper)"/> method
    /// to create and validate instances of this wrapper/proxy class.
    /// </summary>
    /// <param name="target">The pointer to the object.</param>
    private FFOptionsWrapper(void* target)
        : base(target)
    {
    }

    /// <summary>
    /// Gets an empty <see cref="FFOptionsWrapper"/>.
    /// </summary>
    public static FFOptionsWrapper Empty { get; } = new(null);

    /// <summary>
    /// Gets the currently stored options in the object.
    /// Does not include options stored in child objects.
    /// </summary>
    public IReadOnlyList<FFOption> CurrentOptions
    {
        get
        {
            if (IsNull)
                return Array.Empty<FFOption>();

            var options = new List<FFOption>();

            AVOption* currentOption = null;
            do
            {
                currentOption = ffmpeg.av_opt_next(Target, currentOption);
                if (currentOption is not null && currentOption->name is not null)
                    options.Add(new(currentOption));

            } while (currentOption is not null);
            return options;
        }
    }

    /// <summary>
    /// Gets the child options-enabled object currently associated with
    /// this object.
    /// </summary>
    public IReadOnlyList<FFOptionsWrapper> CurrentChildren
    {
        get
        {
            if (IsNull)
                return Array.Empty<FFOptionsWrapper>();

            var result = new List<FFOptionsWrapper>();
            void* currentChild = null;

            do
            {
                currentChild = ffmpeg.av_opt_child_next(Target, currentChild);

                if (currentChild is not null)
                    result.Add(new FFOptionsWrapper(currentChild));

            } while (currentChild is not null);

            return result;

        }
    }

    /// <summary>
    /// Gets the <see cref="INativeReference.Address"/> as a pointer.
    /// </summary>
    private void* Target => Address.ToPointer();

    public static bool TryWrap<T>(INativeReference<T> obj,[MaybeNullWhen(false)] out FFOptionsWrapper optionsObject)
        where T : unmanaged
    {
        optionsObject = null;
        if (obj is null || obj.IsNull) return false;
        if (!CheckIsAVOptionsEnabled<T>())
            return false;

        optionsObject = new(obj.Address.ToPointer());
        return true;
    }

    public bool TryFindOption(string optionName, bool searchChildren, [MaybeNullWhen(false)] out FFOption option)
    {
        option = default;
        if (IsNull)
            return false;

        var searchFlags = (searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : 0);
        var avoption = ffmpeg.av_opt_find(Target, optionName, null, 0, searchFlags);
        if (avoption is not null)
        {
            option = new(avoption);
            return true;
        }

        return false;
    }

    public bool HasOption(string optionName, bool searchChildren) =>
        TryFindOption(optionName, searchChildren, out _);

    public void SetOptionValue(string optionName, bool searchChildren, string value)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(Target, optionName, value, searchFlags);
        FFException.ThrowIfNegative(resultCode, $"Failed to set option value for '{optionName}'.");
    }

    public bool TrySetOptionValue(string optionName, bool searchChildren, string value)
    {
        if (IsNull || string.IsNullOrWhiteSpace(optionName) || string.IsNullOrWhiteSpace(value))
            return false;

        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_set(Target, optionName, value, searchFlags);
        return resultCode == 0;
    }

    public string GetOptionValue(string optionName, bool searchChildren)
    {
        ObjectDisposedException.ThrowIf(IsNull, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(optionName, nameof(optionName));

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(Target, optionName, searchFlags, &stringValuePointer);

        FFException.ThrowIfNegative(resultCode, $"Failed to get option value for '{optionName}'.");
        return NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
    }

    public bool TryGetOptionValue(string optionName, bool searchChildren, [MaybeNullWhen(false)] out string value)
    {
        value = null;
        if (IsNull || string.IsNullOrWhiteSpace(optionName))
            return false;

        byte* stringValuePointer;
        var searchFlags = searchChildren ? ffmpeg.AV_OPT_SEARCH_CHILDREN : default;
        var resultCode = ffmpeg.av_opt_get(Target, optionName, searchFlags, &stringValuePointer);

        if (resultCode == 0)
        {
            value = NativeExtensions.ReadString(stringValuePointer) ?? string.Empty;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the associated media class that defines all possible
    /// options and child, option-enabled objects.
    /// </summary>
    /// <returns>The media class.</returns>
    internal FFMediaClass GetMediaClass()
    {
        if (IsNull)
            return FFMediaClass.Empty;

        var avClassPointer = (AVClass*)((nint*)Target)[0];
        return new(avClassPointer);
    }

    private static bool CheckIsAVOptionsEnabled<TStruct>()
        where TStruct : unmanaged
    {
        lock (SyncRoot)
        {
            if (!MediaClassField.TryGetValue(typeof(TStruct), out var isValid))
            {
                var fields = typeof(TStruct).GetFields();
                isValid = fields.Length > 0 && fields[0].FieldType == typeof(AVClass*);
                MediaClassField[typeof(TStruct)] = isValid;
            }

            return isValid;
        }
    }
}
