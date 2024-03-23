using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class GetDotnetRequest : INativeRequest<GetDotnetResponse>
{
    public NativeRequestCode Code => NativeRequestCode.GET_DOTNET;
    
    [ProtoMember(1)]
    public string Name { get; set; }
}

[ProtoContract]
public class GetDotnetResponse
{
    [ProtoMember(1)]
    public string Email { get; set; }
}
