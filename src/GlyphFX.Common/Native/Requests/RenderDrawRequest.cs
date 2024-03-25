using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RenderDrawRequest : INativeRequest<RenderDrawResponse>
{
    public NativeRequestCode Code => NativeRequestCode.RENDER_DRAW;
}

[ProtoContract]
public class RenderDrawResponse
{
    
}