using ProtoBuf;

namespace GlyphFX.Common.Native;

[ProtoContract]
public class GetPersonRequest : INativeRequest<GetPersonResponse>
{
    [ProtoMember(1)]
    public string Name { get; set; }
    
    public NativeRequestCode Code => NativeRequestCode.GET_PERSON;
}

[ProtoContract]
public class GetPersonResponse
{
    [ProtoMember(1)]
    public string Email { get; set; }
}