using System.Numerics;
using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class BeginRenderRequest : INativeRequest<BeginRenderResponse>
{
    public NativeRequestCode Code => NativeRequestCode.BEGIN_RENDER;
    
    [ProtoMember(1)]
    public byte[] InstanceBuffer { get; set; }
    
    [ProtoMember(2)]
    public byte[] CameraBuffer { get; set; }
}

public struct CameraBuffer(Matrix4x4 viewProjection, Vector3 pos)
{
    public Matrix4x4 ViewProjection { get; set; } = viewProjection;
    public Vector3 Pos { get; set; } = pos;
}

[ProtoContract]
public class BeginRenderResponse
{
    
}