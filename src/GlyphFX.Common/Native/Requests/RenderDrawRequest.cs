using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RenderDrawRequest : INativeRequest<RenderDrawResponse>
{
    [ProtoMember(1)]
    public float[] CameraViewProjection { get; set; }
    [ProtoMember(2)]
    public byte[] InstanceMatrix { get; set; }
    [ProtoMember(3)]
    public uint InstanceCount { get; set; }
    public NativeRequestCode Code => NativeRequestCode.RENDER_DRAW;
}

[ProtoContract]
public class RenderDrawResponse
{
    
}