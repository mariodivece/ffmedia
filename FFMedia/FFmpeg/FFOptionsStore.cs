namespace FFmpeg;

/// <summary>
/// Represents a wrapper for  data structures with AV-options enabled functionality.
/// Such structures must have a <see cref="AVClass"/> pointer as their first defined element.
/// This is a proxy class for implementing the <see cref="IFFOptionsEnabled"/> interface.
/// </summary>
public sealed unsafe class FFOptionsStore :
    NativeReference,
    IFFOptionsEnabled
{
    private static readonly Dictionary<Type, bool> MediaClassField = new(16);
    private static readonly object SyncRoot = new();

    /// <summary>
    /// Creates an instance of the <see cref="FFOptionsStore"/> class.
    /// Use the <see cref="TryWrap{T}(INativeReference{T}, out FFOptionsStore)"/> method
    /// to create and validate instances of this wrapper/proxy class.
    /// </summary>
    /// <param name="target">The pointer to the object.</param>
    private FFOptionsStore(void* target)
        : base(target)
    {
    }

    /// <summary>
    /// Gets an empty <see cref="FFOptionsStore"/>.
    /// </summary>
    public static FFOptionsStore Empty { get; } = new(null);

    /// <inheritdoc />
    public FFMediaClass MediaClass
    {
        get
        {
            if (IsNull)
                return FFMediaClass.Empty;

            var avClassPointer = (AVClass*)((nint*)Target)[0];
            return new(avClassPointer);
        }
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IReadOnlyList<FFOptionsStore> CurrentChildren
    {
        get
        {
            if (IsNull)
                return Array.Empty<FFOptionsStore>();

            var result = new List<FFOptionsStore>();
            void* currentChild = null;

            do
            {
                currentChild = ffmpeg.av_opt_child_next(Target, currentChild);

                if (currentChild is not null)
                    result.Add(new FFOptionsStore(currentChild));

            } while (currentChild is not null);

            return result;

        }
    }

    /// <summary>
    /// Gets the <see cref="INativeReference.Address"/> as a pointer.
    /// </summary>
    private void* Target => Address.ToPointer();

    public static bool TryWrap<T>(INativeReference<T> obj,[MaybeNullWhen(false)] out FFOptionsStore optionsObject)
        where T : unmanaged
    {
        optionsObject = null;
        if (obj is null || obj.IsNull) return false;
        if (!CheckIsAVOptionsEnabled<T>())
            return false;

        optionsObject = new(obj.Address.ToPointer());
        return true;
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
