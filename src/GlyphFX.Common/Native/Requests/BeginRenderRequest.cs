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
    
    [ProtoMember(3)]
    public byte[] LightBuffer { get; set; }
}

public struct CameraBuffer(Matrix4x4 viewProjection, Vector3 pos)
{
    public Matrix4x4 ViewProjection { get; set; } = viewProjection;
    public Vector3 Pos { get; set; } = pos;
}

public struct LightBuffer(Vector3 position, Vector3 color)
{
    public Vector3 Position { get; set; } = position;
    public uint Padding { get; set; } = 0;
    public Vector3 Color { get; set; } = color;
    public uint Padding2 { get; set; } = 0;
}

[ProtoContract]
public class BeginRenderResponse
{
    
}