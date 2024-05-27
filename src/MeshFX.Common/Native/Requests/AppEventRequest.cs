using ProtoBuf;

namespace MeshFX.Common.Native;

[ProtoContract]
public class AppEventRequest : INativeRequest<AppEventResponse>
{
    [ProtoMember(1)]
    public string Name { get; set; }
    public NativeRequestCode Code => NativeRequestCode.APP_EVENT;
}

[ProtoContract]
public class AppEventResponse
{
}