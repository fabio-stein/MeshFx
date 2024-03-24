using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class GetRustRequest : INativeRequest<GetRustResponse>
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    public NativeRequestCode Code => NativeRequestCode.GET_RUST;
}

[ProtoContract]
public class GetRustResponse
{
    [ProtoMember(1)]
    public string Email { get; set; }
}