using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class WindowRedrawRequest : INativeRequest<WindowRedrawResponse>
{
    public NativeRequestCode Code => NativeRequestCode.WINDOW_REDRAW;
}

[ProtoContract]
public class WindowRedrawResponse
{
    
}