using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RenderWaitingRequest : INativeRequest<RenderWaitingResponse>
{
    public NativeRequestCode Code => NativeRequestCode.RENDER_WAITING;
}

[ProtoContract]
public class RenderWaitingResponse
{
    
}