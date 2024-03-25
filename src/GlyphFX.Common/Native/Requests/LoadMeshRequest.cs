using GlyphFX.Common.Native.Primitives;
using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class LoadMeshRequest : INativeRequest<LoadMeshResponse>
{
    [ProtoMember(1)]
    public Vertex[] Vertices { get; set; }
    [ProtoMember(2)]
    public uint[] Indices { get; set; }
    
    public NativeRequestCode Code => NativeRequestCode.LOAD_MESH;
}

[ProtoContract]
public class LoadMeshResponse
{
    
}