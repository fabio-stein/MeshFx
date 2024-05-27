using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class InitRendererRequest : INativeRequest<InitRendererResponse>
{
    public NativeRequestCode Code => NativeRequestCode.INIT_RENDERER;
}

[ProtoContract]
public class InitRendererResponse
{
    
}