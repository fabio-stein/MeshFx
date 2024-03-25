using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class RenderDrawRequest : INativeRequest<RenderDrawResponse>
{
    //4x4 matrix
    [ProtoMember(1)]
    public float[] CameraViewProjection { get; set; }
    //4x4 matrix * instance count
    [ProtoMember(2)]
    public float[] InstanceMatrix { get; set; }
    public NativeRequestCode Code => NativeRequestCode.RENDER_DRAW;
}

[ProtoContract]
public class RenderDrawResponse
{
    
}