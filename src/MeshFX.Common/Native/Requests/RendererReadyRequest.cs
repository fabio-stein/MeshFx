using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class RendererReadyRequest : INativeRequest<RendererReadyResponse>
{
    public NativeRequestCode Code => NativeRequestCode.RENDERER_READY;
}

[ProtoContract]
public class RendererReadyResponse
{
    
}