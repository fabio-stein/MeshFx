namespace GlyphFX.Common.Native;

public interface INativeRequest<T>
{
    NativeRequestCode Code { get; }
}