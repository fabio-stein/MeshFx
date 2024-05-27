using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class LoadMaterialRequest : INativeRequest<LoadMaterialResponse>
{
    public NativeRequestCode Code => NativeRequestCode.LOAD_MATERIAL;
    [ProtoMember(1)]
    public byte[] TextureData { get; set; }
    [ProtoMember(2)]
    public uint MaterialId { get; set; }
}

[ProtoContract]
public class LoadMaterialResponse
{
    
}