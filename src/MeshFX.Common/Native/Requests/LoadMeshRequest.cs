using MeshFX.Common.Native.Primitives;
using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class LoadMeshRequest : INativeRequest<LoadMeshResponse>
{
    [ProtoMember(1)]
    public byte[] Vertices { get; set; }
    [ProtoMember(2)]
    public uint[] Indices { get; set; }
    [ProtoMember(3)]
    public uint MeshId { get; set; }
    
    public NativeRequestCode Code => NativeRequestCode.LOAD_MESH;
}

[ProtoContract]
public class LoadMeshResponse
{
    
}