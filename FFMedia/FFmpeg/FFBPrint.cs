using System.Runtime.InteropServices;

namespace FFmpeg;

/// <summary>
/// Represents a wrapper for the <see cref="AVBPrint"/> structure.
/// This is just a (B)uffer for (Print)ing formatted strings that grows as needed.
/// </summary>
public unsafe class FFBPrint :
    NativeTrackedReferenceBase<AVBPrint>
{
    /// <summary>
    /// Defines the number of bytes to offset from the start of the structure
    /// in order to reach the reserved member.
    /// </summary>
    private static readonly nint ReservedFieldOffset = sizeof(nint) + 3 * sizeof(uint);

    /// <summary>
    /// Initializes a new instance of the <see cref="FFBPrint"/> class.
    /// </summary>
    /// <param name="filePath">The allocation file path.</param>
    /// <param name="lineNumber">The allocation line number.</param>
    public FFBPrint([CallerFilePath] string? filePath = default, [CallerLineNumber] int? lineNumber = default)
        : base(filePath, lineNumber)
    {
        Update(AllocateAutoAVBPrint());
    }

    /// <inheritdoc />
    protected override unsafe void ReleaseInternal(AVBPrint* target)
    {
        var bpStruct = Marshal.PtrToStructure<AVBPrintExtended>((nint)target);
        var reservedAddressPointer = target + ReservedFieldOffset;
        var isAllocated = (nint)reservedAddressPointer != (nint)bpStruct.str;

        if (isAllocated)
            ffmpeg.av_freep(&bpStruct.str);

        ffmpeg.av_freep(&target);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        if (IsNull)
            return string.Empty;

        var bpStruct = Marshal.PtrToStructure<AVBPrintExtended>(Address);
        if (bpStruct.len == 0)
            return string.Empty;

        return NativeExtensions.ReadString(bpStruct.str) ?? string.Empty;
    }

    /// <summary>
    /// Allocates an unmanaged <see cref="AVBPrint"/> structure
    /// that automatically grows as required.
    /// </summary>
    /// <returns>The pointer to the allocated structure.</returns>
    private static unsafe AVBPrint* AllocateAutoAVBPrint()
    {
        // https://ffmpeg.org/doxygen/trunk/bprint_8h-source.html
        const int StructurePadding = 1024;
        var bpStructAddress = ffmpeg.av_mallocz(StructurePadding);
        var bStruct = new AVBPrintExtended
        {
            len = 0,
            size = 1,
            size_max = uint.MaxValue - 1,
            reserved_internal_buffer = 0,
            // point str at the same address of the reserved_internal_buffer
            // which means that the buffer has not been allocated.
            str = (byte*)((nint)bpStructAddress + ReservedFieldOffset)
        };
        
        Marshal.StructureToPtr(bStruct, (nint)bpStructAddress, true);
        return (AVBPrint*)bpStructAddress;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct AVBPrintExtended
    {
        public byte* str;
        public uint len;
        public uint size;
        public uint size_max;
        public byte reserved_internal_buffer;
    }
}
