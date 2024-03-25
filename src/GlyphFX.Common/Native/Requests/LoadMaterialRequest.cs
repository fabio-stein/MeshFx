using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class LoadMaterialRequest : INativeRequest<LoadMaterialResponse>
{
    public NativeRequestCode Code => NativeRequestCode.LOAD_MATERIAL;
}

[ProtoContract]
public class LoadMaterialResponse
{
    
}