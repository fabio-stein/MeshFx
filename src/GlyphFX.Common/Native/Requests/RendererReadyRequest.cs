using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RendererReadyRequest : INativeRequest<RendererReadyResponse>
{
    public NativeRequestCode Code => NativeRequestCode.RENDERER_READY;
}

[ProtoContract]
public class RendererReadyResponse
{
    
}