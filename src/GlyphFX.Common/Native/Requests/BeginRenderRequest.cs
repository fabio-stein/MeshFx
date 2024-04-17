using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class BeginRenderRequest : INativeRequest<BeginRenderResponse>
{
    public NativeRequestCode Code => NativeRequestCode.BEGIN_RENDER;
    
    [ProtoMember(1)]
    public byte[] InstanceBuffer { get; set; }
}

[ProtoContract]
public class BeginRenderResponse
{
    
}