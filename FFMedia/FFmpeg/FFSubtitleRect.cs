namespace FFmpeg;

public unsafe class FFSubtitleRect : NativeReferenceBase<AVSubtitleRect>
{
    public FFSubtitleRect(AVSubtitleRect* target)
        : base(target)
    {

    }

    public AVSubtitleType SubtitleType => Target->type;

    public string? PlainText => NativeExtensions.ReadString(Target->text);

    public string? SsaText => NativeExtensions.ReadString(Target->ass);

    public int Flags => Target->flags;

    public int ColorCount => Target->nb_colors;

    public int Left => Target->x;

    public int Top => Target->y;

    public int Width => Target->w;

    public int Height => Target->h;


}
