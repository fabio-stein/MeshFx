using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class RenderDrawRequest : INativeRequest<RenderDrawResponse>
{
    [ProtoMember(1)]
    public uint InstanceItemOffset { get; set; }
    [ProtoMember(2)]
    public uint InstanceCount { get; set; }
    [ProtoMember(3)]
    public uint MeshId { get; set; }
    [ProtoMember(4)]
    public uint MaterialId { get; set; }
    public NativeRequestCode Code => NativeRequestCode.RENDER_DRAW;
}

[ProtoContract]
public class RenderDrawResponse
{
    
}